using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ConsoleApp1
{
    public class SendEmail
    {
        public static string splitEmail(string email)
        {
            MailAddress emailAddress = new MailAddress(email);
            return emailAddress.User;
        }

        public static void SendEmailTo(string fromEmail, string sender, string toEmail, string code)
        {
            var fromAddress = new MailAddress(fromEmail, sender);
            var toAddress = new MailAddress(toEmail);
            const string appPassword = "zehcrryaxdsvvcpj";
            const string subject = "Code from our love";
            string body = "Your code to access free wifi: " + code;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, appPassword)
            };
            MailMessage message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.Default,
                IsBodyHtml = true
            };
            smtp.Send(message);
        }
    }
}
