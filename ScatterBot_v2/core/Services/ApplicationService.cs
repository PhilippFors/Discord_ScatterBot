using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ScatterBot_v2.core.Services
{
    /// <summary>
    /// Managing the application itself.
    /// </summary>
    public class ApplicationService
    {
        public CancellationTokenSource applicationTerminationToken;
        private string appPath => Directory.GetCurrentDirectory();

        public ApplicationService()
        {
            applicationTerminationToken = new CancellationTokenSource();
            InitProcessId();
        }
    
        public void Cancel()
        {
            applicationTerminationToken.Cancel();
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