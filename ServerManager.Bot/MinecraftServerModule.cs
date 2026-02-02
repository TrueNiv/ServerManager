using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;
using ServerManager.Manager;

namespace ServerManager.Bot;

public class MinecraftServerModule(RestClient client) : IMessageCreateGatewayHandler
{
    public static string BotPrefix;
    public static MinecraftManager? McManager;
    public async ValueTask HandleAsync(Message arg)
    {
        var message = arg.Content.Trim();
        if (ServerVariables.ServerChannel != arg.ChannelId || McManager is null) return;
        
        if (message.Equals($"{BotPrefix}start"))
        {
            if (McManager.Start())
            {
                await client.SendMessageAsync(arg.ChannelId, 
                    $"Starting server under the following ip: {GetPublicIPAddressAsync().Result}");
            }
            else
            {
                await client.SendMessageAsync(arg.ChannelId,
                    $"Server is already running. To get the current IP type {BotPrefix}status.");
            }
        }
        if (message.Equals($"{BotPrefix}stop"))
        {
            if (McManager.Stop())
            {
                await client.SendMessageAsync(arg.ChannelId, $"Stopping server...");
            }
            else
            {
                await client.SendMessageAsync(arg.ChannelId,
                    $"Server is not running. To start the server type {BotPrefix}start.");
            }
        }
        if (message.Equals($"{BotPrefix}status"))
        {
            await client.SendMessageAsync(arg.ChannelId, 
                $"IP: {GetPublicIPAddressAsync().Result}\nServer is running: {McManager.IsRunning()}");
        }
        
        
        if (!ServerVariables.ServerAdmins.Contains(arg.Author.Id)) return;
        
        if (message.StartsWith($"{BotPrefix}command "))
        {
            var length = BotPrefix.Length + "command ".Length;
            var substring = message.Substring(length);
            
            var result = McManager.RunCommand(substring);
            if (result is not null) await client.SendMessageAsync(arg.ChannelId, result);
            else await client.SendMessageAsync(arg.ChannelId, "Command did not reach the server. Check if the server is running.");
        }
    }
    
    
    private static async Task<string> GetPublicIPAddressAsync()
    {
        using (var client = new HttpClient())
        {
            string url = "https://api.ipquery.io";
            var response = await client.GetStringAsync(url);
            return response.Trim();
        }
    }
}