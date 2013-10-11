using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailReceiverTwo.Models
{
    public class PageModel
    {
        public string ValidationSummary { get; set; }
        public List<ErrorModel> Errors { get; set; } 
    }
}