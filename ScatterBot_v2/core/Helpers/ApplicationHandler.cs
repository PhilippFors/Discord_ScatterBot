using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ScatterBot_v2.core.Helpers
{
    public class ApplicationHandler
    {
        public CancellationTokenSource ctx;
        private string appPath => Directory.GetCurrentDirectory();

        public ApplicationHandler()
        {
            ctx = new CancellationTokenSource();
            InitProcessId();
        }
    
        public void Cancel()
        {
            ctx.Cancel();
        }
    
        private void InitProcessId()
        {
            // remember the process id so it can be terminated in console, just in case
            File.CreateText(appPath + "/processId.txt").Close();
            using var writer = File.AppendText(appPath + "/processId.txt");
            writer.Write(Process.GetCurrentProcess().Id.ToString());
        }
    }
}