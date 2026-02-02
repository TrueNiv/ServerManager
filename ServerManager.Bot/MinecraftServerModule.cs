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
        if (ServerVariables.ServerChannel != arg.ChannelId || McManager is null) return;
        
        if (arg.Content.Equals($"{BotPrefix}start"))
        {
            McManager.Start();
            await client.SendMessageAsync(arg.ChannelId, $"Starting server under the following ip: {GetPublicIPAddressAsync().Result}");
        }
        if (arg.Content.Equals($"{BotPrefix}stop"))
        {
            McManager.Stop();
            await client.SendMessageAsync(arg.ChannelId, $"Stopping server...");
        }
        if (arg.Content.Equals($"{BotPrefix}status"))
        {
            await client.SendMessageAsync(arg.ChannelId, $"IP: {GetPublicIPAddressAsync().Result}\nServer is running: {McManager.IsRunning()}");
        }
        
        
        if (!ServerVariables.ServerAdmins.Contains(arg.Author.Id)) return;
        
        if (arg.Content.StartsWith($"{BotPrefix}command "))
        {
            var length = BotPrefix.Length + "command ".Length;
            var substring = arg.Content.Substring(length);
            
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