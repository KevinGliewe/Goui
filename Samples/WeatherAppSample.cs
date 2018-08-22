using Goui;
using Xamarin.Forms;

namespace Samples
{
    public class WeatherAppSample : ISample
    {
        public string Title => "Xamarin.Forms WeatherApp";

        public Goui.Html.Element CreateElement()
        {
            var page = new WeatherApp.WeatherPage();
            return page.GetGouiElement();
        }

        public void Publish()
        {
            UI.Publish("/weatherapp", CreateElement);
        }
    }
}
