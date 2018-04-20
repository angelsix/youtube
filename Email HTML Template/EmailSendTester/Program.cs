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

            var msg = new MailMessage("noreply@fasetto.com", "contact@angelsix.com");

            msg.Subject = "Verify Your Account";
            msg.Body = File.ReadAllText(@"C:\Users\Luke\Desktop\Email\emailtest.txt");
            msg.IsBodyHtml = true;

            var smtpClient = new SmtpClient();
            smtpClient.Credentials = new NetworkCredential(username, password);
            smtpClient.Host = "smtp.office365.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);
        }
    }
}
