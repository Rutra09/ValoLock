using ValoLock.Classes;

namespace ValoLock.Configuration;

public class Configuration
{
    public Dictionary<string, int[]> AgentsLocation { get; set; }
    public string InstaLockMethod { get; set; }
    public bool IsConfigured { get; set; }
    public int[] ButtonLocation { get; set; }
    public String[] UnlockedAgents { get; set; }
}