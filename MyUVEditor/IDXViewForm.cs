using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyUVEditor
{
    public interface IDXViewForm
    {
        DXView ViewPort { get; }
        void Show();
        IntPtr Handle { get; }
        string Text { get; set; }
    }
}
