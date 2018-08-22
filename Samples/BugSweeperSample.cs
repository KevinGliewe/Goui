using Goui;
using Xamarin.Forms;

namespace Samples
{
    public class BugSweeperSample : ISample
    {
        public string Title => "Xamarin.Forms BugSweeper";

        public Goui.Html.Element CreateElement ()
        {
            var page = new BugSweeper.BugSweeperPage ();
            return page.GetGouiElement ();
        }
    }
}
