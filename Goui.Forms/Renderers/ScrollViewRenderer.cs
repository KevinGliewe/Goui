using System;
using System.ComponentModel;
using Goui.Forms.Extensions;
using Xamarin.Forms;

namespace Goui.Forms.Renderers
{
    public class ScrollViewRenderer : VisualElementRenderer<ScrollView>
    {
        bool disposed = false;

        protected override void OnElementChanged (ElementChangedEventArgs<ScrollView> e)
        {
            if (e.OldElement != null) {
                e.OldElement.ScrollToRequested -= Element_ScrollToRequested;
            }

            if (e.NewElement != null) {
                Style.Overflow = "scroll";

                e.NewElement.ScrollToRequested += Element_ScrollToRequested;
            }

            base.OnElementChanged (e);
        }

        protected override void Dispose (bool disposing)
        {
            base.Dispose (disposing);

            if (disposing && !disposed) {
                if (Element != null) {
                    Element.ScrollToRequested -= Element_ScrollToRequested;
                }
                disposed = true;
            }
        }

        void Element_ScrollToRequested (object sender, ScrollToRequestedEventArgs e)
        {
            var oe = (ITemplatedItemsListScrollToRequestedEventArgs)e;
            var item = oe.Item;
            var group = oe.Group;
            if (e.Mode == ScrollToMode.Position) {
                Send (Goui.Message.Set (Id, "scrollTop", e.ScrollY));
                Send (Goui.Message.Set (Id, "scrollLeft", e.ScrollX));
            }
            else {
                switch (e.Position) {
                    case ScrollToPosition.Start:
                        Send (Goui.Message.Set (Id, "scrollTop", 0));
                        break;
                    case ScrollToPosition.End:
                        Send (Goui.Message.Set (Id, "scrollTop", new Goui.Message.PropertyReference { TargetId = Id, Key = "scrollHeight" }));
                        break;
                }
            }
        }
    }
}
