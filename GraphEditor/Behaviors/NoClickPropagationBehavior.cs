using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace GraphEditor.Behaviors;

public class NoClickPropagationBehavior : Behavior<UIElement>
{
    protected override void OnAttached() =>
        AssociatedObject.MouseLeftButtonDown += ClickHandler;

    protected override void OnDetaching() =>
        AssociatedObject.MouseLeftButtonDown -= ClickHandler;

    private void ClickHandler(object _, MouseButtonEventArgs e) => e.Handled = true;
}