using AydaBot.Common.Enums;
using AydaBot.Common.Models;
using System;
using System.Collections.Generic;

namespace AydaBot.Common.Abstraction
{
    public interface IStorage
    {
        void AddOrUpdate(string id, string conversation);
        IEnumerable<NotifyMessage> GetNotifyMessages();
        void SaveSerialMessage(List<Common.Models.SerialMessage> messages);
        List<Common.Models.Serial> GetSerials();
        List<Common.Models.Serial> GetSubscriptions(string userId);
        ErrorCode AddSubscription(Common.Models.Subscription subscription);
        ErrorCode DeleteSubscription(Common.Models.Subscription subscription);
        void UpdateSubscription(Common.Models.Subscription subscription, DateTime dt);
        Statistic GetStatistic();
    }
}
