using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BpmBot.DB.Model
{
    public class Chat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int TelegramId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }

        public virtual List<User> Users { get; set; }
        public virtual List<Result> Results { get; set; }
    }
}