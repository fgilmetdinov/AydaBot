using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AydaBot.SqliteStorage
{
    [Table("Users")]
    public class User
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }
        public string Conversation { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
