using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskYourMechanicDon.Core.Models
{
    public class Order : BaseEntity
    {
        public Order()
        {
            this.Currency = "CAD";
            this.OrderItems = new List<OrderItem>();
        }
        public string CustomerUserId { get; set; }
        public string OrderNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string PayPalTxnId { get; set; }
        public string AmountPaid { get; set; }
        public string Currency { get; set; }

        public Nullable<DateTime> PayPalPaidDate { get; set; }

        public Nullable<DateTime> OrderStatusDate { get; set; }
        public string OrderStatus { get; set; }

        public virtual List<OrderItem> OrderItems { get; set; }
    }

}
