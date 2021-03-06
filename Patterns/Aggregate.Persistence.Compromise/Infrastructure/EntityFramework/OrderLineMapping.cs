﻿using System.Data.Entity.ModelConfiguration;
using Aggregate.Persistence.Compromise.Domain;

namespace Aggregate.Persistence.Compromise.Infrastructure.EntityFramework
{
    public class OrderLineMapping : EntityTypeConfiguration<OrderLine>
    {
        public OrderLineMapping()
        {
            this.ToTable("OrderLine");
            this.HasKey(x => new { x.OrderId, x.Product });
            this.Property(x => x.OrderId);
            this.Property(x => x.Product);
            this.Property(x => x.Quantity);
            this.Property(x => x.CreationDate);
        }
    }
}