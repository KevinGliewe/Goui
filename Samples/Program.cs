﻿using System;
using Goui;
using Goui.Plugin.Maps;

namespace Samples
{
    class Program
    {
        static void Main (string[] args)
        {
            Xamarin.Forms.Forms.Init ();

            UI.Config = new UIConfig() {
                Plugins = new IGouiPlugin[] {
                    new MapsPlugin()
                }
            };
            UI.Port = 8080;
            for (var i = 0; i < args.Length; i++) {
                var a = args[i];
                switch (args[i]) {
                    case "-p" when i + 1 < args.Length:
                    case "--port" when i + 1 < args.Length:
                        {
                            int p;
                            if (int.TryParse (args[i + 1], out p)) {
                                UI.Port = p;
                            }
                            i++;
                        }
                        break;
                }
            }

            new EntryListViewSample().Publish();
            new ButtonSample ().Publish ();
            new TodoSample ().Publish ();
            new DrawSample ().Publish ();
            new FilesSample ().Publish ();
            new DisplayAlertSample ().Publish ();
            new DotMatrixClockSample().Publish();
            new EditorSample().Publish();
            new MonkeysSample().Publish();
            new Navigation.NavigationSample().Publish();
            new RefreshListViewSample ().Publish ();
            new SearchBarSample().Publish();
            new SliderSample().Publish();
            new SwitchListViewSample().Publish();
            new TimePickerSample().Publish();
            new TipCalcSample().Publish();
            new WeatherAppSample().Publish();
            new XuzzleSample().Publish();
            new WebViewSample().Publish();
            new PickerSample().Publish();
            new MapSample().Publish();

            UI.Present ("/display-alert");

            Console.ReadLine ();
        }
    }
}
