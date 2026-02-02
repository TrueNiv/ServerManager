using System.Diagnostics;

namespace ServerManager.Manager;

public class MinecraftManager : IDisposable
{
    private string Path;
    private Process _process;
    private readonly RconHandler _rconHandler;
    
    public MinecraftManager(string path)
    {
        Path = path;
        CreateProcess();
        
        _rconHandler = new RconHandler("127.0.0.1", ServerVariables.RconPort, ServerVariables.RconPassword);
    }

    private void CreateProcess()
    {
        _process = new Process();
        _process.StartInfo.UseShellExecute = false;
        _process.StartInfo.CreateNoWindow = true;

        var startInfo = new ProcessStartInfo
        {
            FileName = Path,
            WorkingDirectory = System.IO.Path.GetDirectoryName(Path)
        };
        _process.StartInfo = startInfo;
    }

    public bool Start()
    {
        if (IsRunning()) return false;
        _process.Start();
        return true;
    }

    public bool Stop()
    {
        if (!IsRunning()) return false;
        _rconHandler.RunCommand("stop").Wait();
        return true;
    }

    public string? RunCommand(string command)
    {
        var task = _rconHandler.RunCommand(command);
        return task.Result;
    }

    public bool IsRunning()
    {
        try
        {
            return !_process.HasExited;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void Dispose()
    {
        _process.Dispose();
    }
}