using System;

namespace AydaBot.Common.Models
{
    public class NotifyMessage
    {        
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public Subscription Subscription { get; set; }
    }
}
