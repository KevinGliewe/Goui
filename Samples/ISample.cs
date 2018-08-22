using System;
using Goui;
using Goui.Html;

namespace Samples
{
    public interface ISample
    {
        string Title { get; }
        Element CreateElement ();
    }
}
