using NLog;

namespace AvaloniaBarebonesV1.NLogMVVM.Interfaces;

public class NLogEntry
{
    public LogLevel? Level { get; set; }
    public string Message { get; set; }
}

public interface ILogTargetObserver
{
    void NewMessage(NLogEntry logMessage);
}