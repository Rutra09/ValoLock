using System.Diagnostics;

namespace ValoLock.Utility
{
    public class ValorantProcessChecker
    {
        public void PrintProcessList()
        {
            foreach (var process in Process.GetProcesses())
            {
                Console.WriteLine(process.ProcessName);
            }
        }
        
        public bool IsValorantRunning()
        {
            return Process.GetProcesses().Any(p => p.ProcessName.Contains("VALORANT"));
        }
    }
}