﻿using System;
using System.Linq;
using Aggregate.Persistence.EventSourcing.Domain;
using Aggregate.Persistence.EventSourcing.Domain.Base;
using Aggregate.Persistence.EventSourcing.Domain.Events;
using Aggregate.Persistence.EventSourcing.Infrastructure.EntityFramework;
using Newtonsoft.Json;

namespace Aggregate.Persistence.EventSourcing.Infrastructure
{
    public class EntityFrameworkOrderRepository : IOrderRepository
    {
        public Order Get(Guid id)
        {
            using var dataContext = new DataContext();
            var domainEvents = dataContext
                .Set<OrderEvent>()
                .Where(x => x.AggregateId == id)
                .OrderBy(x => x.CreationDate)
                .ToArray()
                .Select(ConvertToDomainEvent)
                .ToArray();

            if (domainEvents.OfType<OrderDeleted>().Any()) {
                return null;
            }

            var order = new Order();
            ((IEventPlayer) order).Replay(domainEvents);
            return order;
        }

        public void Add(Order order)
        {
            SaveUncommitedEvents(order);
        }

        public void Update(Order order)
        {
            SaveUncommitedEvents(order);
        }

        public void Delete(Guid orderId)
        {
            var @event = new OrderDeleted(orderId);
            var eventToPersist = ConvertToPersistentEvent(@event);
            using var dataContext = new DataContext();
            dataContext.Set<OrderEvent>().Add(eventToPersist);
            dataContext.SaveChanges();
        }

        // ----- Internal logics
        private static void SaveUncommitedEvents(Order order)
        {
            var domainEvents = order.GetUncommittedEvents();
            var persistedEvents = domainEvents.Select(ConvertToPersistentEvent);
            using var dataContext = new DataContext();
            dataContext.Set<OrderEvent>().AddRange(persistedEvents);
            dataContext.SaveChanges();
        }

        // ----- Utils
        private static OrderEvent ConvertToPersistentEvent(IDomainEvent domainEvent)
        {
            return new OrderEvent {
                AggregateId = domainEvent.AggregateId,
                CreationDate = DateTime.Now,
                Name = domainEvent.GetType().ToString(),
                Content = JsonConvert.SerializeObject(domainEvent)
            };
        }

        private IDomainEvent ConvertToDomainEvent(OrderEvent persistedEvent)
        {
            var type = GetType().Assembly.GetType(persistedEvent.Name);
            return (IDomainEvent) JsonConvert.DeserializeObject(persistedEvent.Content, type);
        }
    }
}