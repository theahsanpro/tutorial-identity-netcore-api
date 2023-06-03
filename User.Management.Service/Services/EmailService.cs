﻿using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Management.Service.Models;

namespace User.Management.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailService(EmailConfiguration emailConfiguration)
        {
            this._emailConfig = emailConfiguration;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        public MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage message)
        {
            using var client = new SmtpClient();

            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                client.Send(message);
            }
            catch
            {
                // Do something
                throw;
            }

            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

    }
}
