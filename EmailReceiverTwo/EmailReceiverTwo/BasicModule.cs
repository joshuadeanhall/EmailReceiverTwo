using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace EmailReceiverTwo
{
    public class BasicModule : NancyModule
    {
        public BasicModule() :base("basic")
        {
            Get["/"] = _ =>
            {
                string x = "hello world";
                return Response.AsText(x);
            };
        }
    }
}