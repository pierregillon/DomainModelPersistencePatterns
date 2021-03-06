using System.Collections.Generic;

namespace Aggregate.Persistence.StateInterface.Domain
{
    public static class CopyExtensions
    {
        public static void CopyTo<TSource, TTarget>(this IOrderStates<TSource> source, IOrderStates<TTarget> target)
            where TSource : IOrderLineStates
            where TTarget : IOrderLineStates, new()
        {
            target.Id = source.Id;
            target.OrderStatus = source.OrderStatus;
            target.SubmitDate = source.SubmitDate;
            target.TotalCost = source.TotalCost;
            target.Lines = source.Lines.Clone<TSource, TTarget>();
        }

        public static void CopyTo(this IOrderLineStates source, IOrderLineStates target)
        {
            target.Product = source.Product;
            target.Quantity = source.Quantity;
            target.CreationDate = source.CreationDate;
        }

        public static ICollection<TTarget> Clone<TSource, TTarget>(this IEnumerable<TSource> source)
            where TSource : IOrderLineStates
            where TTarget : IOrderLineStates, new()
        {
            ICollection<TTarget> target = new List<TTarget>();
            foreach (var orderLine in source) {
                var persistentOrderLine = new TTarget();
                orderLine.CopyTo(persistentOrderLine);
                target.Add(persistentOrderLine);
            }

            return target;
        }
    }
}