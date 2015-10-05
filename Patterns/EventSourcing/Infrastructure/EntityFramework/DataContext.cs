﻿using System.Data.Entity;
using Patterns.Common.Infrastructure;

namespace Patterns.EventSourcing.Infrastructure.EntityFramework
{
    public class DataContext : DataContextBase
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new OrderEventMapping());
        }
    }
}