using System.Collections.Immutable;

namespace ServerManager.Manager;

public static class ServerVariables
{
    public static int RconPort;
    public static string RconPassword;
    public static string ServerFilePath;
    
    public static ulong ServerChannel;
    public static List<ulong> ServerAdmins = [];

    public static void DecipherAdmins(string admins)
    {
        var adminlist = admins.Split(';');
        foreach (var admin in adminlist)
        {
            ulong currentAdmin;
            if (!ulong.TryParse(admin, out currentAdmin))
            {
                Console.WriteLine("The Channel Id of the used channel is invalid. Please set the 'ServerManager_ServerAdmins' environment variable to valid integer Ids seperated by ';'.");
                return;
            }
            ServerAdmins.Add(currentAdmin);
        }
    }
}