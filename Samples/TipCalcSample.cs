using Goui;
using Xamarin.Forms;

namespace Samples
{
    public class TipCalcSample : ISample
    {
        public string Title => "Xamarin.Forms TipCalc";

        public Goui.Html.Element CreateElement()
        {
            var page = new TipCalc.TipCalcPage();
            return page.GetGouiElement();
        }

        public void Publish()
        {
            UI.Publish("/tipcalc", CreateElement);
        }
    }
}
