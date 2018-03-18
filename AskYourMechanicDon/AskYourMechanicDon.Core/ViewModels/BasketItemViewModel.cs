using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskYourMechanicDon.Core.ViewModels
{
    public class BasketItemViewModel
    {
        public string Id { get; set; }
        public int Quanity { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string Vin { get; set; }
        public string Question { get; set; }
    }
}
