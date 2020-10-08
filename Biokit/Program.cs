using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace Biokit
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Press a key to start listening..");
            Console.ReadKey();

            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44367/applicationshub")
                .Build();

            var clientInfo = new ConnectedClient
            {
                AppName = "Biokit",
                AppVersion = "1.0"
            };

            connection.On<ConnectedClient>("UpdateClientInfo", (client) =>
            {
                clientInfo.ConnectionId = client.ConnectionId;
                Console.WriteLine(clientInfo);
            });

            connection.StartAsync().GetAwaiter().GetResult();

            connection.InvokeAsync("GetAppInfo", clientInfo).GetAwaiter().GetResult();

            Console.WriteLine("Listening..... Press a key to quit");
            Console.ReadKey();
        }
    }
}
