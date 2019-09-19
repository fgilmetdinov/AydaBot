using AydaBot.Common.Abstraction;
using AydaBot.Common.Enums;
using AydaBot.Common.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AydaBot.MemoryStorage
{
    public class StorageManager : IStorage
    {
        private readonly ConcurrentDictionary<string, string> _storage;

        public StorageManager()
        {
            _storage = new ConcurrentDictionary<string, string>();
        }

        public void AddOrUpdate(string id, string conversation)
        {
            _storage.AddOrUpdate(id, conversation, (key, newValue) => conversation);
        }

        public IEnumerable<NotifyMessage> GetNotifyMessages()
        {
            throw new System.NotImplementedException();
        }

        public ErrorCode AddSubscription(Subscription subscription)
        {
            throw new System.NotImplementedException();
        }

        public ErrorCode DeleteSubscription(Subscription subscription)
        {
            throw new System.NotImplementedException();
        }

        public List<Serial> GetSerials()
        {
            throw new System.NotImplementedException();
        }

        public List<Serial> GetSubscriptions(string userId)
        {
            throw new System.NotImplementedException();
        }

        public void SaveSerialMessage(List<SerialMessage> messages)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateSubscription(Subscription subscription, DateTime dt)
        {
            throw new NotImplementedException();
        }
    }
}
