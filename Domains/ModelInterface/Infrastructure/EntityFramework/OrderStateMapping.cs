﻿using System.Data.Entity.ModelConfiguration;

namespace Domains.ModelInterface.Infrastructure.EntityFramework
{
    public class OrderStateMapping : EntityTypeConfiguration<OrderPersistantModel>
    {
        public OrderStateMapping()
        {
            this.ToTable("Order");
            this.HasKey(x => x.Id);
            this.Property(x => x.OrderStatus);
            this.Property(x => x.TotalCost);
            this.Property(x => x.SubmitDate);
            this.HasMany(x => x.Lines).WithRequired().HasForeignKey(x=>x.OrderId);
        }
    }
}
