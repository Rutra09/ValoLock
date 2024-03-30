using System;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using ValoLock.Classes;
using ValoLock.Utility;
using Point = System.Drawing.Point;

namespace ValoLock
{
    public partial class AgentLocationSelectWindow : Window
    {
        private String _nowLocatingAgent = "";
        private int _nowLocatingAgentIndex = 0;
        String[] _unlockedAgents = ((App)Application.Current).Agents.UnlockedAgents;
        
        public AgentLocationSelectWindow()
        {
            InitializeComponent();

            this.Left = SystemParameters.WorkArea.Width - this.Width - 100;
            this.Top = 100;

            ((App)Application.Current).MouseHook.MouseAction += MouseHook_MouseAction;
            ((App)Application.Current).MouseHook.Start();
            _nowLocatingAgent = _unlockedAgents[0];
            _nowLocatingAgentIndex = 0;
            UpdateDisplay();
        }

        void UpdateDisplay()
        {
            App app = (App)Application.Current;
            // this.ConfigurationAgentLocationImage.Source = new BitmapImage(new Uri($"/Agents/{Agents.AgentsList[Agents.GetAgentId(_nowLocatingAgent)].Icon}", UriKind.Relative));
            this.ConfigurationAgentLocationImage.Source = new BitmapImage(new Uri($"/Agents/{app.Agents.AgentsList[app.Agents.GetAgentId(_nowLocatingAgent)].Icon}", UriKind.Relative));
            // this.ConfigurationAgentLocationText.Text = $"Please select the location of the {Agents.AgentsList[Agents.GetAgentId(_nowLocatingAgent)].Name} Agent";
            this.ConfigurationAgentLocationText.Text = $"Please select the location of the {_nowLocatingAgent} Agent";
        }

        void ButtonLocationPick()
        {
            ((App)Application.Current).MouseHook.MouseAction -= MouseHook_MouseAction;
            this.ConfigurationAgentLocationImage.Source = new BitmapImage(new Uri("/imgs/lockin.png", UriKind.Relative));
            this.ConfigurationAgentLocationText.Text = "Please select the location of the button";
            ((App)Application.Current).MouseHook.MouseAction += (sender, e) =>
            {
                ((App)Application.Current).ButtonLocation = new[] {e.X, e.Y};
                ((App)Application.Current).SaveAgents();
                ((App)Application.Current).MouseHook.Stop();
                this.Close();
            };
            
        }
        
        void NextAgent()
        {
            _nowLocatingAgentIndex++;
            if (_nowLocatingAgentIndex >= _unlockedAgents.Length)
            {
                ((App)Application.Current).Agents.SaveAgents();
                // this.Close();
                ButtonLocationPick();
                return;
            }

            _nowLocatingAgent = _unlockedAgents[_nowLocatingAgentIndex];
            UpdateDisplay();
        }

        private void MouseHook_MouseAction(object? sender, Point e)
        {
            App app = (App)Application.Current;
            Agent agent = app.Agents.AgentsList[app.Agents.GetAgentId(_nowLocatingAgent)];
            agent.XLocation = e.X;
            agent.YLocation = e.Y;
            NextAgent();
        }
    }
}