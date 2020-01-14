using ComputeAverage;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        const string target = "127.0.0.1:50051";
        const string esc = "esc";

        static async Task Main(string[] args)
        {
            Channel channel = new Channel(target, ChannelCredentials.Insecure);

            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("The client connected successfully");
            });

            var client = new ComputeAverageService.ComputeAverageServiceClient(channel);

            var stream = client.ComputeAverage();

            string key;
            
            do
            {
                Console.WriteLine(Environment.NewLine + "Digit a number or type 'ESC' to exit");
                key = Console.ReadLine();

                var isNumeric = int.TryParse(key, out int number);

                if (key.ToLower() != esc && isNumeric)
                {
                    await stream.RequestStream.WriteAsync(new ComputeAverageRequest()
                    {
                        Number = number
                    });
                }
                
            } while (key.ToLower() != esc);

            await stream.RequestStream.CompleteAsync();

            var responseClientStream = await stream.ResponseAsync;

            Console.WriteLine(responseClientStream.ToString());

            channel.ShutdownAsync().Wait();
            Console.ReadLine();
        }
    }
}
