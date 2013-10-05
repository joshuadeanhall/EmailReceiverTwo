using System;
using Raven.Imports.Newtonsoft.Json;

namespace EmailReceiverTwo.Domain
{
    [JsonObject(IsReference = true)] 
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FriendlyName { get; set; }
        public string EmailAddress { get; set; }
        public string LoginType { get; set; }
        public string Password { get; set; }
        public Organization Organization { get; set; }
    }
}
