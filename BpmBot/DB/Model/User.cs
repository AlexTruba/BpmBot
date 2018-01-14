using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BpmBot.DB.Model
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TelegramId { get; set; }

        public string FirstName{ get; set; }
        public string LastName { get; set; }
        
        public Chat Chat { get; set; }
    }
}
