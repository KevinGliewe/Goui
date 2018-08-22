using Goui.Forms.Extensions;
using System.ComponentModel;
using Xamarin.Forms;

namespace Goui.Forms.Cells
{
    public class TextCellRenderer : CellRenderer
    {
        protected override CellElement CreateCellElement (Cell item)
        {
            return new TextCellElement ();
        }
    }
}
