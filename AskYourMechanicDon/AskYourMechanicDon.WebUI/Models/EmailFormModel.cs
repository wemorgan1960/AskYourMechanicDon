using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AskYourMechanicDon.WebUI.Models
{
    public class EmailFormModel
    {
        [Required, Display(Name ="Your Name")]
        public string FromName { get; set; }

        [Required, Display(Name ="Your email address"), EmailAddress]
        public string FromEmail{get;set;}

        [Required]
        public string Message { get; set; }
    }
}