using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using ValoLock.Classes;
using ValoLock.Configuration;
using ValoLock.Utility;

namespace ValoLock;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public Agents Agents { get; set; } = new(); 
    public bool IsConfigured { get; set; }
    public LockMethod InstaLockMethod { get; set; }
    public ValorantProcessChecker ValorantProcessChecker { get; set; }
    
    public App()
    {
        ValorantProcessChecker = new();
        string json = File.ReadAllText("appsettings.json");
        Configuration.Configuration? configuration = System.Text.Json.JsonSerializer.Deserialize<Configuration.Configuration>(json);
        if (configuration == null) return;
        IsConfigured = configuration.IsConfigured;
        InstaLockMethod = (LockMethod)Enum.Parse(typeof(LockMethod), configuration.InstaLockMethod);
        foreach (var agent in configuration.AgentsLocation)
        {
            Agents.AgentsList[Agents.GetAgentId(agent.Key)].XLocation = agent.Value[0];
            Agents.AgentsList[Agents.GetAgentId(agent.Key)].YLocation = agent.Value[1];
        }
        Agents.UnlockedAgents = configuration.UnlockedAgents.Select(x => Agents.AgentsList[Agents.GetAgentId(x)]).ToArray();
        Console.WriteLine(ValorantProcessChecker.IsValorantRunning());
    }
    
    public void SaveUnlockedAgents()
    {
        Configuration.Configuration configuration = new()
        {
            IsConfigured = IsConfigured,
            InstaLockMethod = InstaLockMethod.ToString(),
            AgentsLocation = Agents.AgentsList.ToDictionary(x => x.Value.Name, x => new Int32[] {x.Value.XLocation, x.Value.YLocation}),
            UnlockedAgents = Agents.UnlockedAgents.Select(x => x.Name).ToArray()
        };
        File.WriteAllText("appsettings.json", System.Text.Json.JsonSerializer.Serialize(configuration));
    }
}