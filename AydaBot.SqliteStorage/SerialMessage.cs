using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AydaBot.SqliteStorage
{
    [Table("SerialMessages")]
    public class SerialMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public int SerialId { get; set; }
        public Serial Serial { get; set; }
    }
}
