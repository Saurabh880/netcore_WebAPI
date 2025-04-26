using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class Message
    {
        public string To  { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public Message(string ToEmail, string subject, string content)
        {
            To  = ToEmail;
            Subject = subject;
            Content = content;
        }
    }
}
