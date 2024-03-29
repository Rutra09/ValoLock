namespace ValoLock.Classes;

public class Agent
{
    public String Name { get; set; }
    public String Icon { get; set; }
    
    public Int32 XLocation { get; set; }
    public Int32 YLocation { get; set; }
    
    public Agent(String name, String icon)
    {
        Name = name;
        Icon = icon;
    }
}