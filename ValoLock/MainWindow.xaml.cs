using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ValoLock.Classes;
using ValoLock.Configuration;

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
           
            
                ReloadAgents();
                this.Buttons.Children.Clear();
                
                Button startButton = new RoundedButton
                {
                    Content = "Rozpocznij InstaLock",
                    Style = (Style)FindResource("BottomButton"),
                    Background = Brushes.LimeGreen,
                    Name = "StartButton"
                };
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
                
                configureButton2.Click += (_, _) =>
                {
                    AgentUnlockedSelectionWindow agentUnlockedSelectionWindow = new();
                    agentUnlockedSelectionWindow.Show();
                    agentUnlockedSelectionWindow.Closed += (_, _) =>
                    {
                        ReloadAgents();
                        this.WindowState = WindowState.Normal;
                        this.Activate();
                    };
                };
                
                configureButton.Click += (_, _) =>
                {
                    if (!((App)Application.Current).ValorantProcessChecker.IsValorantRunning())
                    {
                       // MessageBoxResult result =  MessageBox.Show("Uruchom VALORANT przed lokalizacją agentów \nCzy chcesz uruchomić?", "Bład", MessageBoxButton.OK, MessageBoxImage.Error);
                       MessageBoxResult result = MessageBox.Show("Uruchom VALORANT przed lokalizacją agentów \nCzy chcesz uruchomić?", "Bład", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                       if (result == MessageBoxResult.OK)
                       {
                           ((App)Application.Current).ValorantProcessChecker.StartValorant();
                       } 
                       return;
                    }
                    
                    AgentLocationSelectWindow agentLocationSelectWindow = new();
                    agentLocationSelectWindow.Show();
                    this.WindowState = WindowState.Minimized;
                    ((App)Application.Current).ValorantProcessChecker.MaximizeValorant();
                    agentLocationSelectWindow.Closed += (_, _) =>
                    {
                        ReloadAgents();
                        this.WindowState = WindowState.Normal;
                        this.Activate();
                    };
                };
                startButton.Click += (_, _) =>
                {
                    if (!((App)Application.Current).ValorantProcessChecker.IsValorantRunning())
                    {
                        MessageBoxResult result = MessageBox.Show("Uruchom VALORANT przed rozpoczęciem InstaLocka \nCzy chcesz uruchomić?", "Bład", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                        if (result == MessageBoxResult.OK)
                        {
                            ((App)Application.Current).ValorantProcessChecker.StartValorant();
                        }
                        return;
                    }
                    ((App)Application.Current).IsConfigured = true;
                    ((App)Application.Current).SaveAgents();
                    ReloadAgents();
                    if (GetSelectedAgent() == null)
                    {
                        MessageBox.Show("Wybierz agenta przed rozpoczęciem InstaLocka", "Bład", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    ((App)Application.Current).ValorantProcessChecker.MaximizeValorant();
                    ((App)Application.Current).Instalocking.InstaLockMethod = ((App)Application.Current).InstaLockMethod;
                    ((App)Application.Current).Instalocking.AgentXLocation = GetSelectedAgent().XLocation;
                    ((App)Application.Current).Instalocking.AgentYLocation = GetSelectedAgent().YLocation;
                    ((App)Application.Current).Instalocking.ButtonXLocation = ((App)Application.Current).ButtonLocation[0];
                    ((App)Application.Current).Instalocking.ButtonYLocation = ((App)Application.Current).ButtonLocation[1];
                    new Thread(() =>
                    {
                        ((App)Application.Current).Instalocking.StartLocking();
                    }).Start();
                };

                this.Buttons.Children.Add(startButton);
                this.Buttons.Children.Add(configureButton);
                this.Buttons.Children.Add(configureButton2);
            
        }
        
        Agent GetSelectedAgent()
        {
            foreach (var agent in ((App)Application.Current).Agents.AgentsList)
            {
                var agentObject = FindName(agent.Value.Name);
                if (agentObject is StackPanel agentStackPanel)
                {
                    if (agentStackPanel.Background == Brushes.LightSlateGray)
                    {
                        _agentsInstance.SelectedAgent = agent.Value;
                        return agent.Value;
                    }
                }
            }
            return null!;
        }
        
        void ReloadAgents()
        {
            foreach (var agent in ((App)Application.Current).Agents.AgentsList)
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
            App app = (App)Application.Current;
            UnselectAllAgents();
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            StackPanel agentStackPanel = (StackPanel)sender;
            Int32 agentId = app.Agents.GetAgentId(agentStackPanel.Name);
            Agent agent = app.Agents.AgentsList[agentId];
            agentStackPanel.Background = Brushes.LightSlateGray;
            _agentsInstance.SelectedAgent = agent;
        }

        void LoadAllAgents()
        {
            App app = (App)Application.Current;
            foreach (var agent in app.Agents.AgentsList)
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
            foreach (var agent in ((App)Application.Current).Agents.AgentsList)
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


        private void ImageRecognition_OnClick(object sender, RoutedEventArgs e)
        {
           Button button = (Button)sender;
           if((App)Application.Current == null) return;
           ((App)Application.Current).InstaLockMethod = LockMethod.ScreenRecognition;
           button.Background = new BrushConverter().ConvertFrom("#27374d") as Brush;
           SpamClick.Background = new BrushConverter().ConvertFrom("#395171") as Brush;
        }

        private void SpamClick_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if((App)Application.Current == null) return;
            ((App)Application.Current).InstaLockMethod = LockMethod.SpamClick;
            button.Background = new BrushConverter().ConvertFrom("#27374d") as Brush;
            ImageRecognition.Background = new BrushConverter().ConvertFrom("#395171") as Brush;
        }
        
        
    }
}