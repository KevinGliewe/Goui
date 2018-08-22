using Goui;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Samples
{
    public class MonkeysSample : ISample
    {
        public string Title => "Xamarin.Forms Monkeys";

        public Goui.Html.Element CreateElement()
        {
            var page = new Monkeys.Views.MonkeysView();
            return page.GetGouiElement();
        }

        public void Publish()
        {
            UI.Publish("/monkeys", CreateElement);
        }
    }
}
