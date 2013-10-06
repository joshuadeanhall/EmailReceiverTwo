using System;

namespace EmailReceiverTwo
{
    public class EmailViewModel
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Domain { get; set; }
    }
}