using Goui.Forms.Renderers;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Goui.Forms.Cells
{
    public class ImageCellRenderer : TextCellRenderer
    {
        protected override CellElement CreateCellElement (Cell cell)
        {
            return new ImageCellElement ();
        }
    }
}
