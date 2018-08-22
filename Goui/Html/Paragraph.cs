using System;

namespace Goui.Html {
    public class Paragraph : Element
    {
        public Paragraph ()
            : base ("p")
        {
        }

        public Paragraph (string text)
            : this ()
        {
            Text = text;
        }
    }
}
