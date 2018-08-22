using Goui;
using Xamarin.Forms;

namespace Samples
{
    public class ButtonXamlSample : ISample
    {
        public string Title => "Xamarin.Forms Button XAML";

        public Goui.Html.Element CreateElement ()
        {
            var page = new ButtonXaml.ButtonXamlPage ();
            return page.GetGouiElement ();
        }
    }
}
