using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AydaBot.SqliteStorage
{
    [Table("Subscriptions")]
    public class Subscription
    {
        public int SerialId { get; set; }
        public string UserId { get; set; }
        public DateTime Notified { get; set; }
        
        public Serial Serial { get; set; }        
        public User User { get; set; }

    }
}
