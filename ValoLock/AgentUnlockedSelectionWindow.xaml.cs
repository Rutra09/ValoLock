using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ValoLock.Classes;

namespace ValoLock;

public partial class AgentUnlockedSelectionWindow
{
    private String[] _unlockedAgents;
    
    public AgentUnlockedSelectionWindow()
    {
        App app = (App)Application.Current;
        InitializeComponent();
        _unlockedAgents = app.Agents.UnlockedAgents.ToArray();
        SaveButton.PreviewMouseLeftButtonDown += SaveUnlockedAgents;
        foreach (var agent in app.Agents.AgentsList)
        {
            StackPanel agentStackPanel = new StackPanel
            {
                Name = agent.Value.Name,
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5, 0, 5, 0),
                Background = app.Agents.IsAgentUnlocked(agent.Value.Name) ? Brushes.LightSlateGray : Brushes.Transparent
            };
            Grid agentGrid = new Grid();
            Image agentImage = new Image
            {
                Source = new BitmapImage(new Uri($"/Agents/{agent.Value.Icon}", UriKind.Relative)),
                Height = 100,
                Width = 100
            };
            Image LockImage = new Image
            {
                Source = new BitmapImage(new Uri("/imgs/Lock.png", UriKind.Relative)),
                Height = 100,
                Width = 100,
                Visibility = app.Agents.IsAgentUnlocked(agent.Value.Name) ? Visibility.Hidden : Visibility.Visible
            };
            TextBlock agentName = new TextBlock
            {
                Text = agent.Value.Name,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontFamily = new FontFamily("/Fonts/#Nunito"),
                Foreground = Brushes.White
            };
            
            agentStackPanel.PreviewMouseLeftButtonDown += ToggleUnlockedAgent;
            agentGrid.Children.Add(agentImage);
            agentGrid.Children.Add(LockImage);
            agentStackPanel.Children.Add(agentGrid);
            agentStackPanel.Children.Add(agentName);
            AgentsGrid.Children.Add(agentStackPanel);
        }
    }
    
    void ToggleUnlockedAgent(object sender, MouseButtonEventArgs e)
    {
        if (sender == null) throw new ArgumentNullException(nameof(sender));
        StackPanel agentStackPanel = (StackPanel)sender;
        Int32 agentId = ((App)Application.Current).Agents.GetAgentId(agentStackPanel.Name);
        Agent agent = ((App)Application.Current).Agents.AgentsList[agentId];
        if (agentStackPanel.Background == Brushes.LightSlateGray)
        {
            agentStackPanel.Background = Brushes.Transparent;
            ((Grid) agentStackPanel.Children[0]).Children[1].Visibility = Visibility.Visible;
            _unlockedAgents = _unlockedAgents.Where(val => val != agent.Name).ToArray();
        }
        else
        {
            agentStackPanel.Background = Brushes.LightSlateGray;
            ((Grid) agentStackPanel.Children[0]).Children[1].Visibility = Visibility.Hidden;
            Array.Resize(ref _unlockedAgents, _unlockedAgents.Length + 1);
            _unlockedAgents[^1] = agent.Name;
        }
    }
  
    void SaveUnlockedAgents(object sender, MouseButtonEventArgs e)
    {
        ((App)Application.Current).Agents.UnlockedAgents = _unlockedAgents;
        ((App)Application.Current).Agents.SaveAgents();
        Close();
    }
    
}