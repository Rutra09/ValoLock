using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using ValoLock.Classes;

namespace ValoLock
{
    public partial class MainWindow
    {
        readonly Agents _agentsInstance = ((App)Application.Current).Agents;

        public MainWindow()
        {
            InitializeComponent();
            this.AgentSelectorScrollViewer.PreviewMouseWheel += MainWindow_PreviewMouseWheel;
            LoadAllAgents();
            if (((App)Application.Current).IsConfigured)
            {
                ReloadAgents();
            }
            else
            {
                ReloadAgents();
                this.Buttons.Children.Clear();
                Button configureButton = new RoundedButton
                {
                    Content = "Lokalizuj agentów",
                    Style = (Style)FindResource("BottomButton"),
                    Background = Brushes.DarkOrange,
                    Name = "AgentLocationButton"
                };
                Button configureButton2 = new RoundedButton
                {
                    Content = "Odblokuj agentów",
                    Style = (Style)FindResource("BottomButton"),
                    Background = Brushes.DarkOrange,
                    Name = "AgentUnlockButton"
                };
                
                configureButton2.Click += (sender, args) =>
                {
                    AgentUnlockedSelectionWindow agentUnlockedSelectionWindow = new();
                    agentUnlockedSelectionWindow.Show();
                    agentUnlockedSelectionWindow.Closed += (o, eventArgs) =>
                    {
                        ReloadAgents();
                        this.WindowState = WindowState.Normal;
                        this.Activate();
                    };
                };

                this.Buttons.Children.Add(configureButton);
                this.Buttons.Children.Add(configureButton2);
            }
        }
        
        void ReloadAgents()
        {
            foreach (var agent in Agents.AgentsList)
            {
                var agentObject = FindName(agent.Value.Name);
                // want to know type of agentObject
                if (agentObject != null)
                {
                    if (agentObject is StackPanel agentStackPanel)
                    {
                        agentStackPanel.Visibility = ((App)Application.Current).Agents.IsAgentUnlocked(agent.Value.Name)
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                        agentStackPanel.PreviewMouseLeftButtonDown += SelectAgent;
                    }
                } else
                {
                    Console.WriteLine($"Agent {agent.Value.Name} not found");
                }
            }
        }
        
        void SelectAgent(object sender, MouseButtonEventArgs e)
        {
            UnselectAllAgents();
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            StackPanel agentStackPanel = (StackPanel)sender;
            Int32 agentId = Agents.GetAgentId(agentStackPanel.Name);
            Agent agent = Agents.AgentsList[agentId];
            agentStackPanel.Background = Brushes.LightSlateGray;
            _agentsInstance.SelectedAgent = agent;
        }

        void LoadAllAgents()
        {
            foreach (var agent in Agents.AgentsList)
            { 
                StackPanel agentStackPanel = new()
                {
                    Name = agent.Value.Name,
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5, 0, 5, 0),
                    Background = Brushes.Transparent,
                    Visibility = ((App)Application.Current).Agents.IsAgentUnlocked(agent.Value.Name) ? Visibility.Visible : Visibility.Collapsed
                };
                Image agentImage = new()
                {
                    Source = new BitmapImage(new Uri($"/Agents/{agent.Value.Icon}", UriKind.Relative)),
                    Height = 100,
                    Width = 100
                };
                TextBlock agentName = new()
                {
                    Text = agent.Value.Name,
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontFamily = new FontFamily("/Fonts/#Nunito"),
                    Foreground = Brushes.White
                };
                agentStackPanel.PreviewMouseLeftButtonDown += SelectAgent;
                agentStackPanel.Children.Add(agentImage);
                agentStackPanel.Children.Add(agentName);
                AgentSelectorStackPanel.Children.Add(agentStackPanel);
                AgentSelectorStackPanel.RegisterName(agent.Value.Name, agentStackPanel);
            }
        }

        void UnselectAllAgents()
        {
            foreach (var agent in Agents.AgentsList)
            {
                var agentSelectorScrollViewer = this.AgentSelectorScrollViewer;
                var agentObject = agentSelectorScrollViewer?.FindName(agent.Value.Name);
                if (agentObject is StackPanel agentStackPanel)
                {
                    agentStackPanel.Background = Brushes.Transparent;
                }
            }
        }

        private void MainWindow_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            double scrollAmount = 120;
            if (e.Delta > 0)
            {
                this.AgentSelectorScrollViewer.ScrollToHorizontalOffset(this.AgentSelectorScrollViewer.HorizontalOffset - scrollAmount);
            }
            else
            {
                this.AgentSelectorScrollViewer.ScrollToHorizontalOffset(this.AgentSelectorScrollViewer.HorizontalOffset + scrollAmount);
            }
            e.Handled = true;
        }
        
       
    }
}