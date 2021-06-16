using Library_API.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using static Library_API.Models.EmailModels;

namespace Library_API.Services
{
    public class EmailService
    {
        public ResponseModel SendEmail(EmailModel model)
        {
            try
            {
                var messageSuccessImport = new MimeMessage();
                messageSuccessImport.From.Add(new MailboxAddress("no-reply", "no-reply@usetaylor.com"));
                messageSuccessImport.To.Add(new MailboxAddress(model.Name, model.To));
                messageSuccessImport.Subject = model.Subject;

                messageSuccessImport.Body = new TextPart("plain")
                {
                    Text = model.Message
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.usetaylor.com", 587, SecureSocketOptions.None);
                    client.Authenticate("no-reply@usetaylor.com", "Enourmossauro25");
                    client.Send(messageSuccessImport);
                    client.Disconnect(true);
                }

                return new ResponseModel("E-mail Enviado", 200);
            }
            catch (Exception e)
            {
                return new ResponseModel("Ocorreu um erro", 500);
                throw e;
            }
        }

        public ResponseModel SendEmailAsync(string email, string subject, string content)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("LibNow", "no-reply@usetaylor.com"));
            emailMessage.To.Add(new MailboxAddress(email, email));
            emailMessage.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = content;
            emailMessage.Body = bodyBuilder.ToMessageBody();


            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("smtp.usetaylor.com", 587, MailKit.Security.SecureSocketOptions.None);

                    client.Authenticate("no-reply@usetaylor.com", "Enourmossauro25");

                    client.Send(emailMessage);
                    client.Disconnect(true);
                }
                catch
                {
                    return new ResponseModel("Ocorreu um erro no Envio do E-mail", 409);
                }
            }

            return new ResponseModel("E-mail Enviado", 200);
        }
    }
}
