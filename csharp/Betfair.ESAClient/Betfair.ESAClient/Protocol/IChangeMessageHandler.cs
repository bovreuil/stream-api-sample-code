﻿using Betfair.ESASwagger.Model;

namespace Betfair.ESAClient.Protocol
{
    /// <summary>
    /// This interface abstracts connection & cache implementation.
    /// </summary>
    public interface IChangeMessageHandler
    {
        void OnOrderChange(ChangeMessage<OrderMarketChange> change);
        void OnMarketChange(ChangeMessage<MarketChange> change);
        void OnErrorStatusNotification(StatusMessage message);
    }
}
