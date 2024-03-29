using System.Windows;

namespace ValoLock.Classes;

public class Agents
{
    public static Dictionary<Int32 , Agent> AgentsList { get; set; } = new Dictionary<Int32, Agent>
    {
        {0, new Agent("Astra", "Astra_icon.png")},
        {1, new Agent("Breach", "Breach_icon.png")},
        {2, new Agent("Brimstone", "Brimstone_icon.png")},
        {3, new Agent("Chamber", "Chamber_icon.png")},
        {4, new Agent("Clove", "Clove_icon.png")},
        {5, new Agent("Cypher", "Cypher_icon.png")},
        {6, new Agent("Deadlock", "Deadlock_icon.png")},
        {7, new Agent("Fade", "Fade_icon.png")},
        {8, new Agent("Gekko", "Gekko_icon.png")},
        {9, new Agent("Harbor", "Harbor_icon.png")},
        {10, new Agent("Iso", "Iso_icon.png")},
        {11, new Agent("Jett", "Jett_icon.png")},
        {12, new Agent("KAYO", "KAYO_icon.png")},
        {13, new Agent("Killjoy", "Killjoy_icon.png")},
        {14, new Agent("Neon", "Neon_icon.png")},
        {15, new Agent("Omen", "Omen_icon.png")},
        {16, new Agent("Phoenix", "Phoenix_icon.png")},
        {17, new Agent("Raze", "Raze_icon.png")},
        {18, new Agent("Reyna", "Reyna_icon.png")},
        {19, new Agent("Sage", "Sage_icon.png")},
        {20, new Agent("Skye", "Skye_icon.png")},
        {21, new Agent("Sova", "Sova_icon.png")},
        {22, new Agent("Viper", "Viper_icon.png")},
        {23, new Agent("Yoru", "Yoru_icon.png")}
    };

    public Agent? SelectedAgent { get; set; } = null;

    public Agent[] UnlockedAgents { get; set; } = Array.Empty<Agent>();
    

    public bool IsAgentUnlocked(String AgentName)
    { 
        return UnlockedAgents.Any(e => e.Name == AgentName);
    }

    public void SaveUnlockedAgents()
    {
        ((App)Application.Current).SaveUnlockedAgents();
    }

    public static Int32 GetAgentId(String agentName)
    {
        return AgentsList.FirstOrDefault(x => x.Value.Name == agentName).Key;
    }

}