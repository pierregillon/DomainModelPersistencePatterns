using Domain.Base;

namespace Domains.ModelInterface.Domain
{
    public class OrderLine : IOrderLine, IOrderLineStates
    {
        int IOrderLineStates.Quantity
        {
            get { return Quantity; }
            set { Quantity = value; }
        }
        Product IOrderLineStates.Product
        {
            get { return Product; }
            set { Product = value; }
        }

        public Product Product { get; private set; }
        public int Quantity { get; private set; }

        // ----- Constructor
        public OrderLine()
        {
        }
        public OrderLine(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        // ----- Public methods
        public void IncreaseQuantity(int quantity)
        {
            Quantity += quantity;
        }

        // ----- Overrides
        public override bool Equals(object obj)
        {
            var target = obj as OrderLine;
            if (target == null) {
                return base.Equals(obj);
            }

            return target.Product == Product &&
                   target.Quantity == Quantity;
        }
        public override int GetHashCode()
        {
            unchecked {
                return ((int) Product*397) ^ Quantity;
            }
        }
    }
}