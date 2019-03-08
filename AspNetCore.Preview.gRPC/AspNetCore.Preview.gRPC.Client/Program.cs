using Greet;
using Grpc.Core;
using Primes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Preview.gRPC
{
    public class Program
    {
        const string Not = "not";
        static async Task Main(string[] args)
        {
            // Include port of the gRPC server as an application argument
            var port = args.Length > 0 ? args[0] : "50051";

            var channel = new Channel("localhost:" + port, ChannelCredentials.Insecure);
            //var client = new Greeter.GreeterClient(channel);
            var primeClient = new PrimeCalculator.PrimeCalculatorClient(channel);

            var start = 100_000_000;
            var count = 50_000;
            var taskCount = 100;
            var values = Enumerable.Range(start, count);

            var tasks = new Task[taskCount];
            var currentTaskCount = 0;
            foreach (var num in values)
            {
                tasks[currentTaskCount++] = CheckIsPrimeAsync(primeClient, num);
                if (currentTaskCount == taskCount)
                {
                    await Task.WhenAll(tasks);
                    currentTaskCount = 0;
                }
            }

            await Task.WhenAll(tasks);

            await channel.ShutdownAsync();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task CheckIsPrimeAsync(PrimeCalculator.PrimeCalculatorClient primeClient, int item)
        {
            var result = await primeClient.IsPrimeAsync(new NumberRequest() { Value = item });
            if (result.IsPrime)
                Console.WriteLine($"{item} is {(result.IsPrime ? string.Empty : Not)} prime");
        }
    }
}
