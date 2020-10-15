using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;

namespace Biokit
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Press a key to start listening..");
            Console.ReadKey();

            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json");

                var configuration = builder.Build();

                var url = configuration["hostUrl"];
                var connection = new HubConnectionBuilder()
                    .WithUrl(url,(opts)=>
                    {
                        opts.HttpMessageHandlerFactory = (message) =>
                         {
                             if (message is HttpClientHandler clientHandler)
                                 // bypass SSL certificate
                                 clientHandler.ServerCertificateCustomValidationCallback +=
                                     (sender, certificate, chain, sslPolicyErrors) => { return true; };
                             return message;
                         };
                    })
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Listening..... Press a key to quit");
            Console.ReadKey();
        }
    }
}
