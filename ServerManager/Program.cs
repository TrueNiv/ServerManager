// See https://aka.ms/new-console-template for more information
//https://stackoverflow.com/questions/64079577/c-sharp-create-detached-process-on-linux

using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Rest;
using ServerManager.Bot;

var token = Environment.GetEnvironmentVariable("ServerManager_Token");
if (string.IsNullOrWhiteSpace(token))
{
    Console.WriteLine("The Discord bot token is missing. Please set the 'ServerManager_Token' environment variable.");
    return;
}




var builder = Host.CreateApplicationBuilder(args);
builder.Services
    //.AddDiscordGateway(options => options.Token = token)
    .AddDiscordGateway(options =>
    {
        options.Intents = GatewayIntents.GuildMessages
                          | GatewayIntents.DirectMessages
                          | GatewayIntents.MessageContent
                          | GatewayIntents.DirectMessageReactions
                          | GatewayIntents.GuildMessageReactions;
        options.Token = token;
    })
    .AddGatewayHandlers(typeof(ExampleAddHandler).Assembly)
    .AddApplicationCommands();
var host = builder.Build();

// Add commands from modules
host.AddModules(typeof(ExampleModule).Assembly);

await host.RunAsync();
