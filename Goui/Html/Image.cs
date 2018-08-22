using System;

namespace Goui.Html {
    public class Image : Element
    {
        public string Source
        {
            get => GetStringAttribute ("src", null);
            set => SetAttributeProperty ("src", value);
        }

        public Image ()
            : base ("img")
        {
        }
    }
}
