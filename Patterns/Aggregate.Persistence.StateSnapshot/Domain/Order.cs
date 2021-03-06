﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.Domain;
using Common.Domain.Extensions;

namespace Aggregate.Persistence.StateSnapshot.Domain
{
    public class Order : IOrder, IStateSnapshotable<OrderState>
    {
        private readonly PriceCatalog _catalog = new PriceCatalog();
        private readonly List<OrderLine> _lines = new List<OrderLine>();
        private OrderStatus _orderStatus;

        public Guid Id { get; private set; }
        public DateTime? SubmitDate { get; private set; }
        public double TotalCost { get; private set; }

        // ----- Constructor
        public Order()
        {
            Id = Guid.NewGuid();
        }

        // ----- Public methods
        public void AddProduct(Product product, int quantity)
        {
            CheckIfDraft();
            CheckQuantity(quantity);

            var line = _lines.FirstOrDefault(x => x.Product == product);
            if (line == null) {
                _lines.Add(new OrderLine(product, quantity));
            }
            else {
                line.IncreaseQuantity(quantity);
            }

            ReCalculateTotalPrice();
        }

        public void RemoveProduct(Product product)
        {
            CheckIfDraft();

            var line = _lines.FirstOrDefault(x => x.Product == product);
            if (line != null) {
                _lines.Remove(line);
                ReCalculateTotalPrice();
            }
        }

        public int GetQuantity(Product product)
        {
            var line = _lines.FirstOrDefault(x => x.Product == product);
            if (line == null) {
                return 0;
            }

            return line.Quantity;
        }

        public void Submit()
        {
            CheckIfDraft();
            SubmitDate = DateTime.Now.RoundToSecond();
            _orderStatus = OrderStatus.Submitted;
        }

        // ----- Internal logic
        private void CheckIfDraft()
        {
            if (_orderStatus != OrderStatus.Draft)
                throw new OrderOperationException("The operation is only allowed if the order is in draft state.");
        }

        private void CheckQuantity(int quantity)
        {
            if (quantity < 0) {
                throw new OrderOperationException("Unable to add product with negative quantity.");
            }

            if (quantity == 0) {
                throw new OrderOperationException("Unable to add product with no quantity.");
            }
        }

        private void ReCalculateTotalPrice()
        {
            if (_lines.Count == 0) {
                TotalCost = 0;
            }

            TotalCost = _lines.Sum(x => _catalog.GetPrice(x.Product) * x.Quantity);
        }

        // ----- State Snapshot
        OrderState IStateSnapshotable<OrderState>.TakeSnapshot()
        {
            return new OrderState {
                Id = Id,
                OrderStatus = _orderStatus,
                SubmitDate = SubmitDate,
                TotalCost = TotalCost,
                Lines = _lines.TakeSnapshot<OrderLine, OrderLineState>(x => x.OrderId = Id).ToList()
            };
        }

        void IStateSnapshotable<OrderState>.LoadSnapshot(OrderState snapshot)
        {
            Id = snapshot.Id;
            _orderStatus = snapshot.OrderStatus;
            SubmitDate = snapshot.SubmitDate;
            TotalCost = snapshot.TotalCost;

            _lines.Clear();
            _lines.LoadFromSnapshot(snapshot.Lines);
        }

        #region Overrides with no interest

        public override bool Equals(object obj)
        {
            var target = obj as Order;
            if (target == null) {
                return base.Equals(obj);
            }

            return target.Id == Id &&
                   target._orderStatus == _orderStatus &&
                   target.SubmitDate == SubmitDate &&
                   target.TotalCost == TotalCost &&
                   target._lines.IsEquivalentIgnoringOrderTo(_lines);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return "Order with snapshot pattern";
        }

        #endregion
    }
}