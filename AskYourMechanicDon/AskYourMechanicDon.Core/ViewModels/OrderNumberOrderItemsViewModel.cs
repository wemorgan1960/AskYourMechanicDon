using AskYourMechanicDon.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskYourMechanicDon.Core.ViewModels
{
    public class OrderNumberOrderItemsViewModel
    {
        public string OrderNumber { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
