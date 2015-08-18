﻿using System.Data.Entity.ModelConfiguration;
using Domains.Compromise.Domain;

namespace Domains.Compromise.Infrastructure.EntityFramework
{
    public class OrderMapping : EntityTypeConfiguration<Order>
    {
        public OrderMapping()
        {
            this.ToTable("Order");
            this.HasKey(order => order.Id);
            this.Property(x => x.OrderStatus);
            this.Property(x => x.TotalCost);
            this.Property(x => x.SubmitDate).HasColumnType("datetime2");

            this.HasMany(x => x.Lines).WithRequired(x => x.Order);
        }
    }
}
