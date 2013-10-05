using System;
using Raven.Imports.Newtonsoft.Json;

namespace EmailReceiverTwo.Domain
{
    [JsonObject(IsReference = true)] 
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public UserModel Admin { get; set; }
    }
}