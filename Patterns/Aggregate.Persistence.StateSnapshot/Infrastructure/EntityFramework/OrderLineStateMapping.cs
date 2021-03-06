﻿using System.Data.Entity.ModelConfiguration;
using Aggregate.Persistence.StateSnapshot.Domain;

namespace Aggregate.Persistence.StateSnapshot.Infrastructure.EntityFramework
{
    public class OrderLineStateMapping : EntityTypeConfiguration<OrderLineState>
    {
        public OrderLineStateMapping()
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