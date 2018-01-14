using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BpmBot.DB.Model
{
    public class Result
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        public Chat Chat { get; set; }
        public User User { get; set; }
    }
}
