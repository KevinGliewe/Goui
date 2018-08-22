using System;
using Xamarin.Forms;

namespace Xamarin.Forms
{
    public static class PageExtensions
    {
        public static void Publish (this Xamarin.Forms.Page page, string path)
        {
            Goui.UI.Publish (path, () => page.CreateElement ());
        }

        public static void PublishShared (this Xamarin.Forms.Page page, string path)
        {
            var lazyPage = new Lazy<Goui.Html.Element> ((() => page.CreateElement ()), true);
            Goui.UI.Publish (path, () => lazyPage.Value);
        }

        public static Goui.Html.Element GetGouiElement (this Xamarin.Forms.Page page)
        {
            if (!Xamarin.Forms.Forms.IsInitialized)
                throw new InvalidOperationException ("call Forms.Init() before this");
            
            var existingRenderer = Goui.Forms.Platform.GetRenderer (page);
            if (existingRenderer != null)
                return existingRenderer.NativeView;

            ((IPageController)page).SendAppearing ();

            return CreateElement (page);
        }

        static Goui.Html.Element CreateElement (this Xamarin.Forms.Page page)
        {
            if (!(page.RealParent is Application)) {
                var app = new DefaultApplication ();
                app.MainPage = page;
            }
            var result = new Goui.Forms.Platform ();
            result.SetPage (page);
            return result.Element;
        }

        class DefaultApplication : Application
        {
        }
    }
}
