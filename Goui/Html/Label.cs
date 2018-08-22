using System;

namespace Goui.Html {
    public class Label : Element
    {
        public Element For {
            get => GetAttribute<Element> ("for", null);
            set => SetAttributeProperty ("for", value);
        }

        public Label ()
            : base ("label")
        {
        }

        public Label (string text)
            : this ()
        {
            Text = text;
        }

    }
}
