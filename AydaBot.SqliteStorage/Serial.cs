using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AydaBot.SqliteStorage
{
    [Table("Serials")]
    public class Serial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }

        public ICollection<SerialMessage> SerialMessages { get; set; }
    }
}
