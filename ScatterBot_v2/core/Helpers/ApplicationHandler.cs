using System.Threading;

namespace ScatterBot_v2.core.Helpers;

public class ApplicationHandler
{
    public CancellationTokenSource ctx;

    public ApplicationHandler()
    {
        ctx = new CancellationTokenSource();
    }
    
    public void Cancel()
    {
        ctx.Cancel();
    }
}