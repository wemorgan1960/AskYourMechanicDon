using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskYourMechanicDon.Core.Models
{
    public class Order : BaseEntity
    {
        public Order() {
            this.OrderItems = new List<OrderItem>();
        }

        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string OrderStatus { get; set; }
        public string OrderNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string TotalOrder { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
