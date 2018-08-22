using Goui;
using Xamarin.Forms;

namespace Samples
{
    // From https://github.com/praeclarum/Goui/issues/48
    public class SwitchErrorSample : ISample
    {
        public string Title => "Xamarin.Forms Switch Error";

        public Goui.Html.Element CreateElement ()
        {
            var layout = new StackLayout();
            var label = new Xamarin.Forms.Label
            {
                Text = "Switch state goes here",
                HorizontalTextAlignment = TextAlignment.Center
            };
            var sw = new Switch
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            sw.Toggled += (sender, args) =>
            {
                label.Text = $"Switch state is: {((Switch)sender).IsToggled}";
            };
            layout.Children.Add(label);
            layout.Children.Add(sw);
            return new ContentPage
            {
                Content = layout
            }.GetGouiElement();
        }

        public void Publish()
        {
            UI.Publish ("/switch", CreateElement);
        }
    }
}
