﻿using System;

namespace Patterns.EventSourcing.Domain.Events
{
    public class OrderCreated : IDomainEvent
    {
        public Guid AggregateId { get; private set; }
        public OrderCreated(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}