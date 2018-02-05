// <copyright file="MqttConnection.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Connection
{
    using System;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// Mqtt conection class
    /// </summary>
    public class MqttConnection
    {
        /// <summary>
        /// Mqtt channel
        /// </summary>
        private IModel channel;

        /// <summary>
        /// Mqtt Connection
        /// </summary>
        private IConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqttConnection"/> class
        /// </summary>
        /// <param name="hostName">Hostname of connection</param>
        /// <param name="port">Port of connection</param>
        /// <param name="username">Username for authentication</param>
        /// <param name="password">Password for authentication</param>
        /// <param name="queueName">Queue name</param>
        public MqttConnection(string hostName, int port, string username, string password, string queueName)
        {
            this.HostName = hostName;
            this.Port = port;
            this.Username = username;
            this.Password = password;
            this.QueueName = queueName;
        }

        /// <summary>
        /// Gets or sets hostname
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets QueueName
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Create a mqtt connection
        /// </summary>
        /// <returns> Connection Model </returns>
        public IModel Connect()
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = this.HostName,
                ////UserName = this.Username,
                ////Password = this.Password,
                ////Port = this.Port,
                ////Protocol = Protocols.DefaultProtocol
            };

            this.connection = connectionFactory.CreateConnection();

            this.channel = this.connection.CreateModel();
            
            // Close connection automatically once the last open channel on the connection closes
            this.connection.AutoClose = true;

            return this.channel;
        }

        /// <summary>
        /// Read data event
        /// </summary>
        /// <param name="messageReceivedCallback"> Callback function for read data</param>
        public void ReadDataEvnt(EventHandler<BasicDeliverEventArgs> messageReceivedCallback)
        {
            this.channel.QueueDeclare(queue: this.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += messageReceivedCallback;

            this.channel.BasicConsume(queue: this.QueueName, autoAck: true, consumer: consumer);
        }

        /// <summary>
        /// Closes a mqtt connection
        /// </summary>
        public void Disconnect()
        {
            this.channel.Close();

            try
            {
                this.connection.Close();
            }
            catch
            {
                Console.WriteLine("Exception");
            }
        }
    }
}
