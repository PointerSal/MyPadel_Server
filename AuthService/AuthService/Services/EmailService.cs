using MimeKit;  
using MailKit.Net.Smtp;  
using MailKit.Security;  
using System;

namespace AuthService.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587; 
        private readonly string _smtpUsername = "mypadel.pointer.re@gmail.com"; 
        private readonly string _smtpPassword = "ywie ibzi qdys ndky"; 

        
        public bool SendEmail(string toEmail, string subject, string messageBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("MyPadel", _smtpUsername));
                message.To.Add(new MailboxAddress("", toEmail)); 
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = messageBody };

                using (var client = new SmtpClient())  
                {
                    client.Connect(_smtpServer, _smtpPort, SecureSocketOptions.StartTls); 
                    client.Authenticate(_smtpUsername, _smtpPassword);  
                    client.Send(message);  
                    client.Disconnect(true);  
                }

                return true; 
            }
            catch (Exception ex)
            {
                
                return false; 
            }
        }
    }
}
