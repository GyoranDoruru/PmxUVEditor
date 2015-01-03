using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using SlimDX;
using MyUVEditor.DrivingMode;

namespace MyUVEditor
{
    public class MouseEvent
    {
        private Point mouseDownP = Point.Empty;
        private Point mouseBeforMoveP = Point.Empty;
        private Point mouseAfterMoveP = Point.Empty;

        public Point MouseDowendP { get { return this.mouseDownP; } }
        public Point MouseBeforeMoveP { get { return this.mouseBeforMoveP; } }
        public Point MouseAfterMoveP { get { return this.mouseAfterMoveP; } }

        public void MouseDown(MouseEventArgs e)
        {
            this.mouseDownP = e.Location;
            this.mouseBeforMoveP = e.Location;
            this.mouseAfterMoveP = e.Location;
        }
        public void MouseMove(MouseEventArgs e)
        {
            this.mouseBeforMoveP = this.MouseAfterMoveP;
            this.mouseAfterMoveP = e.Location;
        }
        public void MouseUp(MouseEventArgs e) {  }
    }
}
