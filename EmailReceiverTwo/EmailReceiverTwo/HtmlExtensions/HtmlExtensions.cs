using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmailReceiverTwo.Models;
using Nancy.ViewEngines.Razor;

namespace EmailReceiverTwo.HtmlExtensions
{
    public static class HtmlExtensions
    {
        public static Nancy.ViewEngines.Razor.IHtmlString ValidationMessageFor<T>(this HtmlHelpers<T> helper, List<ErrorModel> errors, string propertyName)
        {
            if (!errors.Any())
                return new NonEncodedHtmlString("");

            string span = String.Empty;

            foreach (var item in errors)
            {
                if (item.Name == propertyName)
                
                {
                    span += "<div class=\"alert alert-danger\">" + item.ErrorMessage + "</div>";
                    break;
                }

            }

            return new NonEncodedHtmlString(span);
        }
    }
}