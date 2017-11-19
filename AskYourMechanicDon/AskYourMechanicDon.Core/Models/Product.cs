using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskYourMechanicDon.Core.Models
{
    public class Product : BaseEntity
    {
        [StringLength(50)]
        [DisplayName("Product Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string VIN { get; set; }
        public string Question { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }

    }
}
