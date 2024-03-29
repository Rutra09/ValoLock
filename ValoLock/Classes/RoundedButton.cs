using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ValoLock.Classes;

public class RoundedButton : Button
{
    public RoundedButton()
    {
        // Style = (Style)FindResource("BottomButton");
        // Background = Brushes.DarkOrange;
        Padding = new Thickness(20, 10, 20, 10);
        Margin = new Thickness(5, 5, 5, 5);
        ControlTemplate buttonTemplate = new ControlTemplate(typeof(Button));
        FrameworkElementFactory border = new(typeof(Border));
        border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
        border.SetValue(Border.BorderThicknessProperty, new Thickness(1, 1,1 ,1));
        border.SetValue(Border.CornerRadiusProperty, new CornerRadius(15));
        border.SetValue(Border.PaddingProperty, new TemplateBindingExtension(Button.PaddingProperty));
        FrameworkElementFactory contentPresenter = new(typeof(ContentPresenter));
        contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
        border.AppendChild(contentPresenter);
        buttonTemplate.VisualTree = border;
        this.Template = buttonTemplate;
    }
    
}