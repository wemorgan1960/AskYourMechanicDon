using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskYourMechanicDon.Core.Models
{
    public class BasketItem:BaseEntity
    {
        public string BasketId { get; set; }
        public string ProductId { get; set; }
        public int Quanity { get; set; }
        public string Vin { get; set; }
        public string Question { get; set; }
    }
}
