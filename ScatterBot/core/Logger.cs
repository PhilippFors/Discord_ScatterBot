using System.Text;
using Discord;

namespace ScatterBot.core;

public class Logger
{
    private string FileName => "ScatterBotLog";
    private string DirectoryName => "/Logs/";
    private string CurrentDirectory => Directory.GetCurrentDirectory();
    private string FullPath => CurrentDirectory + DirectoryName;
    
    private string currentFile;
    
    public async Task CreateFile()
    {
        if (!Directory.Exists(FullPath)) {
            Directory.CreateDirectory(FullPath);
        }
        
        var counter = 0;
        currentFile = FullPath + FileName + counter + ".txt";
        
        bool exists(int c)
        {
            currentFile = FullPath + FileName + c + ".txt";
            return File.Exists(currentFile);
        }

        while (exists(counter)) {
            counter++;
        }

        var writer = File.CreateText(currentFile);
        await writer.WriteAsync("Log created at " + DateTime.Now + "\n");
        writer.Close();
        await writer.DisposeAsync();
    }

    public async Task LogToFile(LogMessage msg)
    {
        var fileStream = File.Open(currentFile, FileMode.Append);
        await fileStream.WriteAsync(Encoding.UTF8.GetBytes($"{DateTime.Now}: {msg.ToString()}\n"));
        fileStream.Close();
        await fileStream.DisposeAsync();
    }
    
    public Task LogToConsole(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}