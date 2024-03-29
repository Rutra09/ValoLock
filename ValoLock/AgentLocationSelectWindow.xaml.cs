using System.Windows;

namespace ValoLock;

public partial class AgentLocationSelectWindow : Window
{
    public AgentLocationSelectWindow()
    {
        InitializeComponent();
        
        this.Left = SystemParameters.WorkArea.Width - this.Width - 100;
        this.Top = 100;
    }
}