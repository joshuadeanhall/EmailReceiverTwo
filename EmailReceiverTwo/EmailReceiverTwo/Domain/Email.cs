using System;

namespace EmailReceiverTwo.Domain
{
    public class Email
    {
        public Guid Id { get; set; }
        public Organization Organization { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime Create { get; set; }
        public bool Processed { get; set; }
    }
}