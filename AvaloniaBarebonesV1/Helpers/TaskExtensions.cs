using System.Threading.Tasks;

namespace VisualTests.Helpers;

public static class TaskExtensions
{
    // https://www.meziantou.net/fire-and-forget-a-task-in-dotnet.htm
    public static void FireAndForget(this Task task)
    {
        if (!task.IsCompleted || task.IsFaulted)
        {
            _ = ForgetAwaited(task);
        }
    }
    
    private static async Task ForgetAwaited(Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch
        {
            // Nothing to do here
        }
    }
}
