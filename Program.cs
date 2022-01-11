using System;
using System.Threading.Tasks;
using MQTTnet.Client;

namespace MQTTManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter tag of this endpoint: ");
            var tag = Console.ReadLine();

            MqttManager manager = new MqttManager(tag);

            bool running = true;
            while (running)
            {
                Console.WriteLine("Enter message to publish (subsribe with 1<tag> and 0 to exit)");
                var input = Console.ReadLine();

                if (input.Equals("0"))
                {
                    running = false;
                }
                else if (input[0].Equals('1'))
                {
                    var newTag = input.Remove(0, 1);
                    Console.WriteLine(newTag);
                    manager.SubscribeTag(newTag);
                }
                else
                {
                    manager.PublishMessageAsync(input);
                }
            }

            manager.Disconnect();
        }
    }
}