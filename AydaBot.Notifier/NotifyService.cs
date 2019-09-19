using AydaBot.Common.Models;
using Microsoft.Bot.Builder;
using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Newtonsoft.Json;
using Microsoft.Bot.Schema;

namespace AydaBot.Notifier
{
    public class NotifyService
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _applicationId;
        private readonly Common.Abstraction.IStorage _storage;
        private readonly ILogger<NotifyService> _logger;

        public NotifyService(IBotFrameworkHttpAdapter adapter, string applicationId,
            Common.Abstraction.IStorage storage,
            ILogger<NotifyService> logger)
        {
            _adapter = adapter;
            _applicationId = applicationId;
            _storage = storage;
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation("NotifyService started.");

            var currentMinute = DateTime.Now.Minute;
            while (true)
            {
                if (DateTime.Now.Minute != currentMinute)
                {
                    currentMinute = DateTime.Now.Minute;
                    SendMessages();
                }
                Thread.Sleep(new TimeSpan(0, 1, 0));
            }
        }

        private void SendMessages()
        {
            try
            {
                foreach (var message in _storage.GetNotifyMessages())
                {
                    var conversationReference = JsonConvert.DeserializeObject<ConversationReference>(message.Subscription.Conversation);

                    var task = ((BotAdapter)_adapter)
                        .ContinueConversationAsync(_applicationId, conversationReference, CreateCallback(message.Message), default(CancellationToken));
                    task.Wait();

                    _storage.UpdateSubscription(message.Subscription, message.Date);

                    _logger.LogInformation($"NotifyService send message: {message.Message} to {conversationReference.User.Name ?? conversationReference.User.Id}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }
        }

        private BotCallbackHandler CreateCallback(string message)
        {
            return async (turnContext, token) =>
            {
                await turnContext.SendActivityAsync(message);
            };
        }
    }
}
