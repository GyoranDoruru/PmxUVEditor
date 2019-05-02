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
        public RectSelector(IPXPmx pmx, Camera.ICamera camera)
        {
            Camera = camera;
            Pmx = pmx;
        }
        public int[] Selected { get; set; }

        public Camera.ICamera Camera { get; set; }

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
            if (e.Button != MouseButtons.Left)
                return;
            UpPoint = e.Location;
            var dp = Camera.ScreenToWorldVec(DownPoint);
            var up = Camera.ScreenToWorldVec(UpPoint);
            var min_u = System.Math.Min(dp.X, up.X);
            var min_v = System.Math.Min(dp.Y, up.Y);
            var max_u = System.Math.Max(dp.X, up.X);
            var max_v = System.Math.Max(dp.Y, up.Y);
            if (m_filter == null)
                return;
            var selected = new List<int>(Pmx.Vertex.Count);
            foreach (var vi in m_filter)
            {
                var v = Pmx.Vertex[vi];
                bool is_in = min_u <= v.UV.U && v.UV.U <= max_u
                    && min_v <= v.UV.V && v.UV.V <= max_v;
                if (is_in)
                    selected.Add(vi);
            }
            Selected = selected.ToArray();
            IsChanged = true;
        }

        public void setFilter(IEnumerable<int> filter)
        {
            m_filter = filter;
            Selected = null;
            IsChanged = true;
        }


        IEnumerable<int> m_filter = null;
        private Point DownPoint { get; set; }
        private Point UpPoint { get; set; }

        public bool IsChanged { get; set; }
    }

}
