using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskYourMechanicDon.Core.Models
{
    public class Request:BaseEntity
    {
        public string Message { get; set; }
        public string IsErrored { get; set; }
    }
}
