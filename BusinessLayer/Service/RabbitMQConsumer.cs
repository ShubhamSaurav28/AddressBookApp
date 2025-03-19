using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using ModelLayer.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BusinessLayer.Service
{
    public class RabbitMQConsumer
    {
        private readonly IConfiguration _config;
        private readonly IEmailServiceBL _emailServiceBL;

        public RabbitMQConsumer(IConfiguration config, IEmailServiceBL emailServiceBL)
        {
            _config = config;
            _emailServiceBL = emailServiceBL;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:Host"],
                UserName = _config["RabbitMQ:Username"],
                Password = _config["RabbitMQ:Password"]
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: _config["RabbitMQ:QueueName"], durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var emailMessage = JsonConvert.DeserializeObject<EmailMessageDTO>(message);

                await _emailServiceBL.SendEmailAsync(emailMessage.To, emailMessage.Subject, emailMessage.Body);
            };

            channel.BasicConsume(queue: _config["RabbitMQ:QueueName"], autoAck: true, consumer: consumer);
        }
    }
}

