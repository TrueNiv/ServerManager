using RconSharp;

namespace ServerManager.Manager;

public class RconHandler
{
    private string _ip;
    private int _port;
    private string _password;
    
    public RconHandler(string ip, int port, string password)
    {
        _ip = ip;
        _port = port;
        _password = password;
    }

    public async Task<string?> RunCommand(string command)
    {
        try
        {
            var client = RconClient.Create(_ip, _port);
            await client.ConnectAsync();
            await client.AuthenticateAsync(_password);
            var result = await client.ExecuteCommandAsync(command);
            client.Disconnect();
            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }
}