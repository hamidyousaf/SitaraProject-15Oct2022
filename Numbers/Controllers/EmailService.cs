using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using MailKit.Net.Smtp;
//using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
//using System.Net.Mail;

using MailKit.Security;
using MailKit.Net.Smtp;

namespace Numbers.Controllers
{
    public interface IEmailService
    {
       public  void Send(string from, string to, string subject, string html);
    }
    public class EmailService : IEmailService
    {
        public void Send(string from, string to, string subject, string message)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Plain) { Text = message };
            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 465, true);
            //smtp.Connect("plesk6100.is.cc", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("SajidTalib74@gmail.com", "Sajid111786#");
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        //public void Send(string from, string to, string subject, string message)
        //{

        //    MailMessage msgMail = new MailMessage();

        //    MailMessage myMessage = new MailMessage();
        //    myMessage.From = new MailAddress("websitecontact@tradetoppers.com", "TradeToppers");
        //    myMessage.To.Add(to);
        //    myMessage.Subject = subject;
        //    myMessage.IsBodyHtml = true;

        //    myMessage.Body = message;

        //    System.Net.Mail.SmtpClient mySmtpClient = new System.Net.Mail.SmtpClient();
        //    System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential("websitecontact@tradetoppers.com", "Pakistan786#");
        //    mySmtpClient.Host = "plesk6100.is.cc";
        //    mySmtpClient.UseDefaultCredentials = false;
        //    mySmtpClient.Credentials = myCredential;
        //    mySmtpClient.ServicePoint.MaxIdleTime = 1;

        //    mySmtpClient.Send(myMessage);
        //    myMessage.Dispose();

        //}

    }
}
