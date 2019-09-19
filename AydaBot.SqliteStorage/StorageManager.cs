using AydaBot.Common.Abstraction;
using AydaBot.Common.Enums;
using AydaBot.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace AydaBot.SqliteStorage
{
    public class StorageManager : IStorage
    {
        public StorageManager()
        {
        }

        public void AddOrUpdate(string id, string conversation)
        {
            return;
        }

        public IEnumerable<NotifyMessage> GetNotifyMessages()
        {
            using (var context = new DataContext())
            {
                context.Database.EnsureCreated();

                return context.Subscriptions
                    .Join(context.SerialMessages, s => s.SerialId, m => m.SerialId, (s, m) => new { s, m })
                    .Where(x => x.s.Notified < x.m.Date)
                    .Select(x => new NotifyMessage
                    {
                        Date = x.m.Date,
                        Message = x.m.Message,
                        Subscription = new Common.Models.Subscription
                        {
                            SerialId = x.s.SerialId,
                            UserId = x.s.UserId,
                            Conversation = x.s.User.Conversation,
                        }
                    })
                    .ToList();
            }
        }

        public void UpdateSubscription(Common.Models.Subscription subscription, DateTime dt)
        {
            using (var context = new DataContext())
            {
                var subscriptionDb = context.Subscriptions.Where(x => x.SerialId == subscription.SerialId && x.UserId == subscription.UserId).FirstOrDefault();
                if (subscriptionDb != null)
                {
                    subscriptionDb.Notified = dt;
                    context.SaveChanges();
                }
            }
        }

        public void SaveSerialMessage(List<Common.Models.SerialMessage> messages)
        {
            using (var context = new DataContext())
            {
                context.Database.EnsureCreated();

                foreach (var message in messages)
                {
                    var serial = context.Serials.Where(x => x.Name == message.SerialName).FirstOrDefault();
                    if (serial == null)
                    {
                        serial = new Serial
                        {
                            Name = message.SerialName,
                            Name2 = message.SerialName2
                        };
                        context.Serials.Add(serial);
                        context.SaveChanges();
                    }
                    var serialMessage = context.SerialMessages.Where(x => x.SerialId == serial.Id).FirstOrDefault();
                    if (serialMessage == null)
                    {
                        serialMessage = new SerialMessage
                        {
                            SerialId = serial.Id,
                            Message = message.Message,
                            Date = message.Date
                        };
                        context.SerialMessages.Add(serialMessage);
                    }
                    else if (serialMessage.Date < message.Date)
                    {
                        serialMessage.Date = message.Date;
                        serialMessage.Message = message.Message;
                    }
                    context.SaveChanges();
                }
            }
        }

        public List<Common.Models.Serial> GetSerials()
        {
            using (var context = new DataContext())
            {
                return context.Serials.Select(x => new Common.Models.Serial
                {
                    Id = x.Id,
                    Name = x.Name,
                    Name2 = x.Name2
                })
                    .OrderBy(x => x.Name)
                    .ToList();
            }
        }

        public List<Common.Models.Serial> GetSubscriptions(string userId)
        {
            using (var context = new DataContext())
            {
                return context.Subscriptions.Where(x => x.UserId == userId)
                    .Select(x => new Common.Models.Serial
                    {
                        Id = x.Serial.Id,
                        Name = x.Serial.Name,
                        Name2 = x.Serial.Name2
                    })
                    .OrderBy(x => x.Name)
                    .ToList();
            }
        }

        public ErrorCode AddSubscription(Common.Models.Subscription subscription)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var serial = context.Serials.Where(x => x.Id == subscription.SerialId).FirstOrDefault();
                    if (serial == null)
                        return ErrorCode.SerialNotFound;

                    var user = context.Users.Where(x => x.Id == subscription.UserId).FirstOrDefault();
                    if (user == null)
                    {
                        user = new User
                        {
                            Id = subscription.UserId,
                            Conversation = subscription.Conversation
                        };
                        context.Users.Add(user);

                        context.SaveChanges();
                    }

                    var subscriptionDb = context.Subscriptions.Where(x => x.SerialId == serial.Id && x.UserId == subscription.UserId).FirstOrDefault();
                    if (subscriptionDb != null)
                        return ErrorCode.SubscriptionAlreadyExists;
                    subscriptionDb = new Subscription
                    {
                        SerialId = serial.Id,
                        UserId = user.Id,
                        Notified = SqlDateTime.MinValue.Value
                    };
                    context.Subscriptions.Add(subscriptionDb);

                    context.SaveChanges();

                    return ErrorCode.Ok;
                }
            }
            catch
            {
                return ErrorCode.Error;
            }
        }

        public ErrorCode DeleteSubscription(Common.Models.Subscription subscription)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var serial = context.Serials.Where(x => x.Id == subscription.SerialId).FirstOrDefault();
                    if (serial == null)
                        return ErrorCode.SerialNotFound;

                    var user = context.Users.Where(x => x.Id == subscription.UserId).FirstOrDefault();
                    if (user == null)
                    {
                        user = new User
                        {
                            Id = subscription.UserId
                        };
                        context.Users.Add(user);
                        context.SaveChanges();
                    }

                    var subscriptionDb = context.Subscriptions.Where(x => x.SerialId == serial.Id && x.UserId == subscription.UserId).FirstOrDefault();
                    if (subscriptionDb == null)
                        return ErrorCode.SuscriptionNotFound;
                    context.Subscriptions.Remove(subscriptionDb);

                    context.SaveChanges();

                    return ErrorCode.Ok;
                }
            }
            catch
            {
                return ErrorCode.Error;
            }
        }

        public Statistic GetStatistic()
        {
            var result = new Statistic();
            using (var context = new DataContext())
            {
                result.UserCount = context.Users.Count();
                result.SerialCount = context.Serials.Count();
                result.MaxDate = context.SerialMessages.Max(x => x.Date);
            }
            return result;
        }

    }
}
