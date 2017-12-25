using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using PEPlugin.Pmx;
namespace MyUVEditor.Selector
{
    public interface ISelector
    {
        int[] Selected { get; }
        IPXPmx Pmx { get; set; }
        void ConnectControl(Control control);
        void DisconnectControl(Control control);
        void Clear();
    }
}
