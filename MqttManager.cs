using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace MQTTManager
{
    public class MqttManager
    {
        private static IMqttClient _client;
        private static string _tag;

        public MqttManager(string tag)
        {
            Setup(tag);
        }
        
        public static async Task Setup(string tag)
        {
            _tag = tag;
            
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder().WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer("test.mosquitto.org", 1883).WithCleanSession().Build();
            
            _client.UseConnectedHandler(async e =>
            {
                Console.WriteLine("Connected to MQTT Broker successfully");
            });
            
            _client.UseDisconnectedHandler(e => { Console.WriteLine("Disconnected from MQTT Broker successfully"); });
            
            _client.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine($"Received Message: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            });
            
            await _client.ConnectAsync(options);
        }
        
        public async Task PublishMessageAsync(string text)
        {
            string payload = $"\nFrom: {_tag}\nMessage: {text}";
            var message = new MqttApplicationMessageBuilder().WithTopic(_tag).WithPayload(payload)
                .WithAtLeastOnceQoS().Build();

            if (_client.IsConnected)
            {
                await _client.PublishAsync(message);
            }
        }

        public async Task SubscribeTag(string tag)
        {
            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic(tag)
                .Build();

            await _client.SubscribeAsync(topicFilter);
        }

        public async Task Disconnect()
        {
            Console.WriteLine("Starting to disconnect...");
            await _client.DisconnectAsync();
        }
    }
}