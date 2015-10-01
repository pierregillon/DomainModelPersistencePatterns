using System;
using Domain.Base;

namespace Domains.EventSourcing.Domain.Events
{
    public class ProductRemoved : IDomainEvent
    {
        public Guid AggregateId { get; set; }
        public Product Product { get; set; }

        public ProductRemoved(Guid id, Product product)
        {
            AggregateId = id;
            Product = product;
        }
    }
}