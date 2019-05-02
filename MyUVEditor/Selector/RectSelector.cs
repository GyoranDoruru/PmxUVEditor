using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PEPlugin.Pmx;
using System.Drawing;

namespace MyUVEditor.Selector
{
    class RectSelector : ISelector
    {
        public int[] Selected => throw new NotImplementedException();

        public IPXPmx Pmx { get; set; }

        public void ConnectControl(Control control)
        {
            control.MouseDown += MouseDownEvent;
            control.MouseUp += MouseUpEvent;
        }

        public void DisconnectControl(Control control)
        {
            control.MouseDown -= MouseDownEvent;
            control.MouseUp -= MouseUpEvent;
        }

        void MouseDownEvent(object sendor, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
                DownPoint = e.Location;
        }

        void MouseUpEvent(object sendor, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                UpPoint = e.Location;

        }

        private Point DownPoint { get; set; }
        private Point UpPoint { get; set; }


    }

}
