using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace EmailSendTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var username = "noreply@fasetto.com";
            var password = "somepassword";

            var msg = new MailMessage("noreply@fasetto.com", "contact@angelsix.com")
            {
                Subject = "Verify Your Account",
                Body = File.ReadAllText(@"C:\Users\Luke\Desktop\email.txt"),
                IsBodyHtml = true
            };

            var smtpClient = new SmtpClient
            {
                Credentials = new NetworkCredential(username, password),
                Host = "smtp.office365.com",
                Port = 587,
                EnableSsl = true
            };

            smtpClient.Send(msg);
        }
    }
}
