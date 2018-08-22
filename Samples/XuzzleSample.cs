using Goui;
using Xamarin.Forms;

namespace Samples
{
    public class XuzzleSample : ISample
    {
        public string Title => "Xamarin.Forms Xuzzle";

        public Goui.Html.Element CreateElement()
        {
            var page = new Xuzzle.XuzzlePage();
            return page.GetGouiElement();
        }

        public void Publish()
        {
            UI.Publish("/xuzzle", CreateElement);
        }
    }
}
