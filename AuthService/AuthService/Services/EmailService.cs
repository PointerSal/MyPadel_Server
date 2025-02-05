using MimeKit;  // MimeMessage and other classes
using MailKit.Net.Smtp;  // For SmtpClient (MailKit)
using MailKit.Security;  // For SecureSocketOptions
using System;

namespace AuthService.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587; // Use 465 for SSL
        private readonly string _smtpUsername = "mypadel.pointer.re@gmail.com"; // Your Gmail email
        private readonly string _smtpPassword = "ywie ibzi qdys ndky"; // Replace with your generated app password

        // Send email method
        public bool SendEmail(string toEmail, string subject, string messageBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("MyPadel", _smtpUsername));
                message.To.Add(new MailboxAddress("", toEmail)); // Recipient email
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = messageBody };

                using (var client = new SmtpClient())  // Use MailKit's SmtpClient
                {
                    client.Connect(_smtpServer, _smtpPort, SecureSocketOptions.StartTls); // Connect method of MailKit
                    client.Authenticate(_smtpUsername, _smtpPassword);  // Authenticate with Gmail using App Password
                    client.Send(message);  // Send the email
                    client.Disconnect(true);  // Disconnect after sending
                }

                return true; // Email sent successfully
            }
            catch (Exception ex)
            {
                // Log or handle exception
                return false; // Email sending failed
            }
        }
    }
}
