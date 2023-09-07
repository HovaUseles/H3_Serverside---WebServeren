using H3_Serverside___WebServeren.Webserver;
using H3_Serverside___WebServeren.Webserver.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<IResponseHandler, ResponseHandler>();
        services.AddScoped<IRequestHandler, RequestHandler>();
        services.AddSingleton<IWebServer, WebServer>();
    })
    .Build();


var server = host.Services.GetService<IWebServer>();

IPAddress iPAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
int port = 27016;
int maxCon = 2;
string path = "test/test";

server.Start(iPAddress, port, maxCon, path);
Console.ReadLine();