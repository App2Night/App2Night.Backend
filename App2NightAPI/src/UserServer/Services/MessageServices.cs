using App2Night.Shared;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserServer.Services;

namespace UserServer.Services
{
    public class AuthMessageSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            //Create Mail Object and fill FROM, TO, SUBJECT and Body
            var mailToSend = new MimeMessage();
            mailToSend.From.Add(new MailboxAddress("App2Night", new Secrets().RegisterEmailAdress));
            mailToSend.To.Add(new MailboxAddress(email));
            mailToSend.Subject = subject;
            var htmlMessageBody = new BodyBuilder();
            htmlMessageBody.HtmlBody = message;
            mailToSend.Body = htmlMessageBody.ToMessageBody();

            //Send Mail Object
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.live.com", 587);
                client.Authenticate(new Secrets().RegisterEmailAdress, new Secrets().EmailPasswd);

                client.Send(mailToSend);
                client.Disconnect(true);
            }


            return Task.FromResult(0);
        }
    }
}
