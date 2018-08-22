using System;
using Goui;
using Xamarin.Forms;

namespace Samples
{
    public class DisplayAlertSample : ISample
    {
        public string Title => "Xamarin.Forms DisplayAlert";

        public Goui.Html.Element CreateElement ()
        {
            var page = new DisplayAlertPage ();
            return page.GetGouiElement ();
        }

        public void Publish ()
        {
            UI.Publish ("/display-alert", CreateElement);
        }
    }
}
