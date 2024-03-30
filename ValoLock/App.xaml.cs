using System.IO;
using ValoLock.Classes;
using ValoLock.Configuration;
using ValoLock.Instalocking;
using ValoLock.Utility;

namespace ValoLock;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public Agents Agents { get; set; } 
    public bool IsConfigured { get; set; }
    public int[] ButtonLocation { get; set; }
    public LockMethod InstaLockMethod { get; set; }
    public ValorantProcessChecker ValorantProcessChecker { get; set; }
    public MouseHook MouseHook { get; set; } // Add this line
    public InstaLocking Instalocking { get; set; }
    
    public App()
    {
        this.Instalocking = new();
        ValorantProcessChecker = new();
        string json = File.ReadAllText("appsettings.json");
        Configuration.Configuration? configuration = System.Text.Json.JsonSerializer.Deserialize<Configuration.Configuration>(json);
        if (configuration == null) return;
        IsConfigured = configuration.IsConfigured;
        InstaLockMethod = (LockMethod)Enum.Parse(typeof(LockMethod), configuration.InstaLockMethod);
        int i = 0;
        Agents = new Agents(configuration.AgentsLocation.ToDictionary(x=> i, x =>
        {
            Agent agent = new(x.Key, x.Key + "_icon.png");
            agent.XLocation = x.Value[0];
            agent.YLocation = x.Value[1];
            i++;
            return agent;
        }));
        ButtonLocation = configuration.ButtonLocation;
        Agents.UnlockedAgents = configuration.UnlockedAgents;
        Console.WriteLine(ValorantProcessChecker.IsValorantRunning());
        MouseHook = new MouseHook(); 

    }

    protected override void OnActivated(EventArgs e)
    {
        this.Instalocking.StopLocking();
        base.OnActivated(e);
    }

    public void SaveAgents()
    {
        Configuration.Configuration configuration = new()
        {
            IsConfigured = IsConfigured,
            InstaLockMethod = InstaLockMethod.ToString(),
            AgentsLocation = Agents.AgentsList.ToDictionary(x => x.Value.Name, x => new[] {x.Value.XLocation, x.Value.YLocation}),
            UnlockedAgents = Agents.UnlockedAgents,
            ButtonLocation = ButtonLocation
        };
        File.WriteAllText("appsettings.json", System.Text.Json.JsonSerializer.Serialize(configuration));
    }
    

}