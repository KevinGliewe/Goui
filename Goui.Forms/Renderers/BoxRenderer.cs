using System;
using System.ComponentModel;
using Goui.Forms.Extensions;
using Xamarin.Forms;

namespace Goui.Forms.Renderers
{
    public class BoxRenderer : VisualElementRenderer<BoxView>
    {
        Goui.Color _colorToRenderer;

        protected override void OnElementChanged (ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged (e);

            if (Element != null)
                SetBackgroundColor (Element.BackgroundColor);
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);
            if (e.PropertyName == BoxView.ColorProperty.PropertyName)
                SetBackgroundColor (Element.BackgroundColor);
        }

        protected override void SetBackgroundColor (Xamarin.Forms.Color color)
        {
            if (Element == null)
                return;

            _colorToRenderer = Element.Color.ToGouiColor (Colors.Clear);

            Style.BackgroundColor = _colorToRenderer;
        }
    }
}
