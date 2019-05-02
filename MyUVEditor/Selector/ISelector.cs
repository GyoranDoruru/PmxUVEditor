using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using PEPlugin.Pmx;
namespace MyUVEditor.Selector
{
    internal interface ISelector
    {
        int[] Selected { get; }
        bool IsChanged { get; set; }
        IPXPmx Pmx { get; set; }
        void ConnectControl(Control control);
        void DisconnectControl(Control control);
        Camera.ICamera Camera { get; set; }
        void setFilter(IEnumerable<int> filter);

    }
}
