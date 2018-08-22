using Goui;
using Xamarin.Forms;

namespace Samples
{
    public class DotMatrixClockSample : ISample
    {
        public string Title => "Xamarin.Forms DoMatrixClock";

        public Goui.Html.Element CreateElement()
        {
            var page = new DotMatrixClock.DotMatrixClockPage();
            return page.GetGouiElement();
        }

        public void Publish()
        {
            UI.Publish("/dotmatrixclock", CreateElement);
        }
    }
}
