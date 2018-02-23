using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskYourMechanicDon.Core.Models
{
    public class OrderItem : BaseEntity
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quanity { get; set; }
        public string Vin { get; set; }
        public string Question { get; set; }
        [Display(Name = "Answer Subject")]
        public string AnswerSubject { get; set; }
        [Display(Name = "Answer")]
        public string AnswerContent { get; set; }
        public string AnswerTags { get; set; }
        [Display(Name = "Answer Completed")]
        public Nullable<DateTime> AnswerCompleted { get; set; }
    }
}
