﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.Domain;
using Common.Domain.Extensions;

namespace Aggregate.Persistence.InnerClass.Domain
{
    public class Order : IOrder
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
            return "Order with inner class pattern";
        }

        #endregion

        public class FromState
        {
            public Order Build(OrderState orderState)
            {
                var order = new Order {
                    Id = orderState.Id,
                    _orderStatus = orderState.OrderStatus,
                    SubmitDate = orderState.SubmitDate,
                    TotalCost = orderState.TotalCost,
                };

                order._lines.Clear();
                order._lines.AddRange(new OrderLine.FromState().Build(orderState.Lines));

                return order;
            }
        }

        public class ToState
        {
            public OrderState Build(Order order)
            {
                return new OrderState {
                    Id = order.Id,
                    OrderStatus = order._orderStatus,
                    SubmitDate = order.SubmitDate,
                    TotalCost = order.TotalCost,
                    Lines = new OrderLine.ToState().Build(order._lines, x => x.OrderId = order.Id).ToList()
                };
            }
        }
    }
}