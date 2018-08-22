﻿using Goui;
using Xamarin.Forms;

namespace Samples
{
    public class TimePickerSample : ISample
    {
        public string Title => "Xamarin.Forms TimePicker Sample";

        public Goui.Html.Element CreateElement()
        {
            var panel = new StackLayout();

            var titleLabel = new Xamarin.Forms.Label
            {
                Text = "TimePicker",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold
            };

            panel.Children.Add(titleLabel);

            var timePicker = new TimePicker();
            panel.Children.Add(timePicker);

            var page = new ContentPage
            {
                Content = panel
            };

            return page.GetGouiElement();
        }

        public void Publish()
        {
            UI.Publish("/timepicker", CreateElement);
        }
    }
}
