using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

namespace BusinessLayer.Service
{
    public class EmailServiceBL : IEmailServiceBL
    {
        private readonly IConfiguration _configuration;

        public EmailServiceBL(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            Console.WriteLine("This is the EmailService Body"+body);
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("UserRegistration", _configuration["SmtpSettings:Username"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = body };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(
                    _configuration["SmtpSettings:Host"],
                    int.Parse(_configuration["SmtpSettings:Port"]),
                    bool.Parse(_configuration["SmtpSettings:EnableSsl"])
                );

                await client.AuthenticateAsync(
                    _configuration["SmtpSettings:Username"],
                    _configuration["SmtpSettings:Password"]
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            Console.WriteLine("This is the EmailService Sent");
        }
    }
}

