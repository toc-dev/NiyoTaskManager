using Microsoft.AspNetCore.SignalR.Client;

namespace NiyoTaskManager.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7012/taskHub")
            .Build();

            connection.On<string>("ReceiveTaskUpdate", (message) =>
            {
                Console.WriteLine($"New task update: {message}");
            });

            await connection.StartAsync();
            Console.WriteLine("Connected to SignalR hub.");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            await connection.StopAsync();
        }
    }
}
