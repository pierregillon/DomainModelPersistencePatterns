using System;
using Common.Domain;

namespace Aggregate.Persistence.NoPersistence.Domain
{
    public class OrderLine : IOrderLine
    {
        private readonly DateTime _creationDate;

        public Product Product { get; }
        public int Quantity { get; private set; }

        // ----- Constructor
        public OrderLine(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
            _creationDate = DateTime.Now;
        }

        // ----- Public methods
        public void IncreaseQuantity(int quantity)
        {
            Quantity += quantity;
        }

        #region Overrides with no interest

        public override bool Equals(object obj)
        {
            var target = obj as OrderLine;
            if (target == null) {
                return base.Equals(obj);
            }

            return target.Product == Product &&
                   target.Quantity == Quantity &&
                   target._creationDate == _creationDate;
        }

        public override int GetHashCode()
        {
            unchecked {
                int hashCode = _creationDate.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) Product;
                hashCode = (hashCode * 397) ^ Quantity;
                return hashCode;
            }
        }

        #endregion
    }
}