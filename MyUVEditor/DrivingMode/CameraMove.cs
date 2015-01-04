using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyUVEditor.DrivingMode
{
    class CameraMove : Mode
    {
        private static CameraMove instance = new CameraMove();
        private CameraMove() { }
        public static CameraMove GetInstance() { return instance; }

        public override DrawingMode Type() { return DrawingMode.CAMERAMOVE; }


        public override void MouseDowend(MyGame sender, MouseEventArgs e) { }

        public override void MouseMoved(MyGame sender, MouseEventArgs e)
        {
            MouseEvent me = sender.Mouse;
            bool a = KeyBoardEvent.GetInstance().IsShift;
            sender.Camera.MoveTarget(sender.Device, me.MouseAfterMoveP, me.MouseBeforeMoveP, KeyBoardEvent.GetInstance().IsShift);
        }

        public override void MouseUpped(MyGame sender, MouseEventArgs e)
        {
            sender.drivingMode = Nothing.GetInstance();
        }
    }
}
