using System;

namespace Goui
{
    public class TextNode : Node
    {
        string text = "";
        public override string Text {
            get => text;
            set => SetProperty (ref text, value ?? "", "data");
        }

        public TextNode ()
            : base ("#text")
        {
        }

        public TextNode (string text)
            : this ()
        {
            Text = text;
        }

#if !NO_XML

        public override void WriteOuterHtml (System.Xml.XmlWriter w)
        {
            w.WriteString (text);
        }

#endif
    }
}
