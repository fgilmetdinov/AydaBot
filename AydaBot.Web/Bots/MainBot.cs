using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AydaBot.Common;
using AydaBot.Common.Enums;
using AydaBot.Common.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AydaBot.Web.Bots
{
    public class MainBot : ActivityHandler
    {
        // Message to send to users when the bot receives a Conversation Update event
        private const string WelcomeMessage = "Добро пожаловать в Ayda бот. Введите /help для подсказки.";

        private Common.Abstraction.IStorage _storage;
        private readonly ILogger _logger;

        public MainBot(Common.Abstraction.IStorage storage, ILogger<MainBot> logger)
        {
            _storage = storage;
            _logger = logger;
        }

        private void AddConversationReference(Activity activity)
        {
            var conversationReference = activity.GetConversationReference();
            _storage.AddOrUpdate(conversationReference.User.Id, JsonConvert.SerializeObject(conversationReference));
        }

        protected override Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            AddConversationReference(turnContext.Activity as Activity);

            return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(WelcomeMessage), cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var activity = turnContext.Activity as Activity;
            //AddConversationReference(activity); 
            var conversation = activity.GetConversationReference();

            _logger.LogInformation($"Type: {activity.Type}. Message: {activity.Text} User: {conversation.User.Id}");
            
            switch (activity.Type)
            {
                case "message":
                    var text = activity.Text;
                    var serialId = 0;
                    if (text.StartsWith("/add") || text.StartsWith("/del"))
                    {
                        text = int.TryParse(text.Substring(5).Trim(), out serialId) ? text.Substring(0, 4) : "";                                                
                    }
                    switch (text)
                    {
                        case "/help":
                            await SendMessageAsync(turnContext, "/list - список сериалов\n\r/subscriptions - список подписанных сериалов\n\r/add <номер> - подписаться на сериал\n\r/del <номер> - удалить подписку на сериал", cancellationToken);
                            break;
                        case "/list":
                            var serialMessage = SerialsToString(_storage.GetSerials());
                            
                            await SendMessageAsync(turnContext, serialMessage, cancellationToken);
                            await SendMessageAsync(turnContext, "/add <номер> - подписаться на сериал", cancellationToken);
                            break;
                        case "/subscriptions":                            
                            var subscriptionMessgae = SerialsToString(_storage.GetSubscriptions(conversation.User.Id));

                            await SendMessageAsync(turnContext, subscriptionMessgae, cancellationToken);
                            await SendMessageAsync(turnContext, "/del <номер> - удалить подписку на сериал", cancellationToken);
                            break;
                        case "/add":
                            var addResult = _storage.AddSubscription(new Subscription
                            {
                                UserId = conversation.User.Id,
                                SerialId = serialId,
                                Conversation = JsonConvert.SerializeObject(conversation)
                            });
                            await SendMessageAsync(turnContext, $"Результат: {Helper.GetDescription(addResult)}", cancellationToken);
                            break;
                        case "/del":
                            var delResult = _storage.DeleteSubscription(new Subscription
                            {
                                UserId = conversation.User.Id,
                                SerialId = serialId,
                                Conversation = JsonConvert.SerializeObject(conversation)
                            });
                            await SendMessageAsync(turnContext, $"Результат: {Helper.GetDescription(delResult)}", cancellationToken);
                            break;
                        case "/stat":
                            var statistics = _storage.GetStatistic();
                            await SendMessageAsync(turnContext, $"Users: {statistics.UserCount}. Serials: {statistics.SerialCount}. Max date: {statistics.MaxDate.ToString("dd.MM.yyyy")}", cancellationToken);
                            break;
                        default:
                            await SendDefaultMessageAsync(turnContext, cancellationToken);
                            break;
                    }
                    break;

                default:
                    await SendDefaultMessageAsync(turnContext, cancellationToken);
                    break;
            }
        }

        private async Task SendDefaultMessageAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await SendMessageAsync(turnContext, $"Вы ввели: '{turnContext.Activity.Text}'. Для помощи введите '/help'", cancellationToken);
        }

        private async Task SendMessageAsync(ITurnContext<IMessageActivity> turnContext, string text, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(text), cancellationToken);
        }

        private string SerialsToString(List<Serial> serials) 
        {
            if (serials.Count == 0)
                return "Список пуст";

            var sb = new StringBuilder();
            serials.ForEach(x => sb.AppendLine($"{x.Id} {x.Name} {(string.IsNullOrWhiteSpace(x.Name2) ? "" : string.Concat("(", x.Name2, ")"))}"));
            return sb.ToString();
        }
}
}
