using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyUVEditor.DrivingMode
{
    public abstract class Mode:IDrivingMode
    {
        public void GetDrivingMode(MyGame sender,MouseEventArgs e)
        {
            Mode ret;
            //if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Right) ret = CameraMove.GetInstance();
            if (sender.form.TabControll.SelectedIndex == 1) ret = Nothing.GetInstance();
            else if (sender.form.IsScaleState) ret = Scale.GetInstance();
            else if (sender.form.IsRotateState) ret = Rotate.GetInstance();
            else if (KeyBoardEvent.GetInstance().OnKeys[(int)Keys.ShiftKey]) ret = Add.GetInstance();
            else if (KeyBoardEvent.GetInstance().OnKeys[(int)Keys.ControlKey]) ret = Remove.GetInstance();
            else if (sender.vS.GetNearSelected(sender.mypmx, sender.Camera, e.Location,4) != -1) ret = Move.GetInstance();
            else if (sender.vS.GetNearUsed(sender.mypmx, sender.Camera, e.Location,4) != -1)
            {
                sender.vS.selectedVertexIndex = new int[] {
                    sender.vS.GetNearUsed(sender.mypmx, sender.Camera, e.Location,4)
                };
                ret = Move.GetInstance();
            }
            else ret = Select.GetInstance();
            sender.drivingMode = ret;
        }

        public abstract DrawingMode Type();

        public abstract void MouseDowend(MyGame sender, MouseEventArgs e);
        public abstract void MouseMoved(MyGame sender, MouseEventArgs e);
        public abstract void MouseUpped(MyGame sender, MouseEventArgs e);
    }
}
