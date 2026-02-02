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
using ServerManager.Manager;




var token = Environment.GetEnvironmentVariable("ServerManager_Token");
if (string.IsNullOrWhiteSpace(token))
{
    Console.WriteLine("The Discord bot token is missing. Please set the 'ServerManager_Token' environment variable.");
    return;
}

var prefix = Environment.GetEnvironmentVariable("ServerManager_BotPrefix");
if (string.IsNullOrWhiteSpace(prefix))
{
    Console.WriteLine("The Discord bot prefix. Please set the 'ServerManager_Token' environment variable.");
    return;
}
MinecraftServerModule.BotPrefix = prefix;

var runserverfile = Environment.GetEnvironmentVariable("ServerManager_RunServerShellFile");
if (string.IsNullOrWhiteSpace(runserverfile))
{
    Console.WriteLine("The adress to the server starting file is missing. Please set the 'ServerManager_RunServerShellFile' environment variable.");
    return;
}
ServerVariables.ServerFilePath = runserverfile;

var rconport = Environment.GetEnvironmentVariable("ServerManager_RconPort");
if (string.IsNullOrWhiteSpace(rconport))
{
    Console.WriteLine("The RCON port is missing. Please set the 'ServerManager_RconPort' environment variable.");
    return;
}
if (!int.TryParse(rconport, out var port))
{
    Console.WriteLine("The RCON port is invalid. Please set the 'ServerManager_RconPort' environment variable to a valid integer port number.");
    return;
}
ServerVariables.RconPort = port;

var rconpassword = Environment.GetEnvironmentVariable("ServerManager_RconPassword");
if (string.IsNullOrWhiteSpace(rconpassword))
{
    Console.WriteLine("The RCON password is missing. Please set the 'ServerManager_RconPassword' environment variable.");
    return;
}
ServerVariables.RconPassword = rconpassword;

var serverchannel = Environment.GetEnvironmentVariable("ServerManager_ServerChannel");
if (string.IsNullOrWhiteSpace(serverchannel))
{
    Console.WriteLine("The Channel Id of the used channel is missing. Please set the 'ServerManager_ServerChannel' environment variable.");
    return;
}
if (!ulong.TryParse(serverchannel, out var channel))
{
    Console.WriteLine("The Channel Id of the used channel is invalid. Please set the 'ServerManager_ServerChannel' environment variable to a valid integer Id.");
    return;
}
ServerVariables.ServerChannel = channel;

var serverAdmins = Environment.GetEnvironmentVariable("ServerManager_ServerAdmins");
if (string.IsNullOrWhiteSpace(serverchannel))
{
    Console.WriteLine("The List of admins is missing. Please set the 'ServerManager_ServerAdmins' environment variable.");
    return;
}
ServerVariables.DecipherAdmins(serverAdmins);


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
    .AddGatewayHandlers(typeof(MinecraftServerModule).Assembly)
    .AddApplicationCommands();
var host = builder.Build();

// Add commands from modules
host.AddModules(typeof(ExampleModule).Assembly);

MinecraftServerModule.McManager = new MinecraftManager(ServerVariables.ServerFilePath);
await host.RunAsync();
