﻿using System;

namespace Patterns.Common.Domain
{
    public interface IRepository<TOrder>
    {
        TOrder Get(Guid id);
        void Add(TOrder order);
        void Update(TOrder order);
        void Delete(Guid orderId);
    }
}