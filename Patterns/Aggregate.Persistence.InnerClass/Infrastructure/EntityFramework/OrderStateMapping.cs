﻿using System.Data.Entity.ModelConfiguration;
using Aggregate.Persistence.InnerClass.Domain;

namespace Aggregate.Persistence.InnerClass.Infrastructure.EntityFramework
{
    public class OrderStateMapping : EntityTypeConfiguration<OrderState>
    {
        public OrderStateMapping()
        {
            this.ToTable("Order");
            this.HasKey(x => x.Id);
            this.Property(x => x.OrderStatus);
            this.Property(x => x.TotalCost);
            this.Property(x => x.SubmitDate);
            this.HasMany(x => x.Lines).WithRequired().HasForeignKey(x => x.OrderId);
        }
    }
}