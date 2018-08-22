using System;

namespace Goui.Html {
    public class List : Element
    {
        public List (bool ordered = false)
            : base (ordered ? "ol" : "ul")
        {
        }
    }
}
