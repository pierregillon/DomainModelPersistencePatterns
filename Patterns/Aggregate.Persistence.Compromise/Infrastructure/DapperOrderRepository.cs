﻿using System;
using System.Data.SqlClient;
using System.Linq;
using Aggregate.Persistence.Compromise.Domain;
using Common.Infrastructure;
using Dapper;

namespace Aggregate.Persistence.Compromise.Infrastructure
{
    public class DapperOrderRepository : IOrderRepository
    {
        public Order Get(Guid id)
        {
            using var connection = new SqlConnection(SqlConnectionLocator.LocalhostSqlExpress());
            const string query = SqlQueries.SelectOrdersByIdQuery + " " + SqlQueries.SelectOrderLinesByIdQuery;
            using var multi = connection.QueryMultiple(query, new { id });
            var order = multi.Read<Order>().SingleOrDefault();
            if (order != null) {
                order.Lines = multi.Read<OrderLine>().ToList();
            }

            return order;
        }

        public void Add(Order order)
        {
            using var connection = new SqlConnection(SqlConnectionLocator.LocalhostSqlExpress());
            connection.Execute(SqlQueries.InsertOrderQuery, order);
            connection.Execute(SqlQueries.InsertOrderLineQuery, order.Lines);
        }

        public void Update(Order order)
        {
            using var connection = new SqlConnection(SqlConnectionLocator.LocalhostSqlExpress());
            connection.Execute(SqlQueries.UpdateOrderQuery, order);
            connection.Execute(SqlQueries.DeleteOrderLineQuery, new { OrderId = order.Id });
            connection.Execute(SqlQueries.InsertOrderLineQuery, order.Lines);
        }

        public void Delete(Guid orderId)
        {
            using var connection = new SqlConnection(SqlConnectionLocator.LocalhostSqlExpress());
            connection.Execute(SqlQueries.DeleteOrderLineQuery, new { OrderId = orderId });
            connection.Execute(SqlQueries.DeleteOrderQuery, new { OrderId = orderId });
        }
    }
}