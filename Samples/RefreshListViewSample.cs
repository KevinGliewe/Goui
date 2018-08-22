using Goui;
using Xamarin.Forms;

namespace Samples
{
    public class RefreshListViewSample : ISample
    {
        public string Title => "Xamarin.Forms RefreshListView";

        public Goui.Html.Element CreateElement ()
        {
            var page = new RefreshListView();
            return page.GetGouiElement ();
        }

        public void Publish ()
        {
            UI.Publish ("/refreshlistview", CreateElement);
        }
    }
}
