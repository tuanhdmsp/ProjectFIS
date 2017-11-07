using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace SplashPageWebApp.Services
{
    public class SendEmailWithTemplate
    {
        public static void SendTo(string fromEmail, string sender, string toEmail, string code)
        {
            string body = CreateEmailBody(code);
            SendEmailTo(fromEmail, sender, toEmail, body);
        }

        public static string CreateEmailBody(string code)
        {
            string body;
            using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath("/EmailTemplate/index.html") ?? throw new InvalidOperationException()))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Code}", code);
            return body;
        }

        public static void SendEmailTo(string fromEmail, string sender, string toEmail, string bodyTemplate)
        {
            var fromAddress = new MailAddress(fromEmail, sender);
            var toAddress = new MailAddress(toEmail);
            const string appPassword = "zehcrryaxdsvvcpj";
            const string subject = "Free Code To Access Wi-Fi";
            string body = bodyTemplate;

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