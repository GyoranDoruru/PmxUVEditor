using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyUVEditor.DrivingMode
{
    class Nothing:Mode
    {
        private static Nothing instance = new Nothing();
        private Nothing() { }
        public static Nothing GetInstance() { return instance; }

        public override DrawingMode Type()
        {
            return DrawingMode.NOTHING;
        }

        public override void MouseDowend(MyGame sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        public override void MouseMoved(MyGame sender, System.Windows.Forms.MouseEventArgs e)
        {
            sender.vS.GetNearUsed(sender.mypmx, sender.Camera, e.Location, 4);
        }

        public override void MouseUpped(MyGame sender, System.Windows.Forms.MouseEventArgs e)
        {
        }
    }
}
