using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace ServerManager.Bot;

public class ExampleAddHandler(RestClient client) : IReadyGatewayHandler
{
    //public async ValueTask HandleAsync(MessageReactionAddEventArgs args)
    //{
    //    await client.SendMessageAsync(args.ChannelId, $"<@{args.UserId}> reacted with {args.Emoji.Name}!");
    //}

    public ValueTask HandleAsync(ReadyEventArgs arg)
    {
        var guild = client.GetGuildAsync(0);
        Console.WriteLine(guild.Result.Name);
        
        return ValueTask.CompletedTask;
    }
}