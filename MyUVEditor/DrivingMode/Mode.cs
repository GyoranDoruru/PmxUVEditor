using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyUVEditor.DrivingMode
{
    public abstract class Mode:IDrivingMode
    {
        public void GetDrivingMode(MyGame mygame,MouseEventArgs e)
        {
            Mode ret;
            if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Right) ret = CameraMove.GetInstance();
            else if (mygame.form.TabControll.SelectedIndex == 1) ret = Nothing.GetInstance();
            else if (mygame.form.IsScaleState) ret = Scale.GetInstance();
            else if (mygame.form.IsRotateState) ret = Rotate.GetInstance();
            else if (KeyBoardEvent.GetInstance().OnKeys[(int)Keys.ShiftKey]) ret = Add.GetInstance();
            else if (KeyBoardEvent.GetInstance().OnKeys[(int)Keys.ControlKey]) ret = Remove.GetInstance();
            else if (mygame.vS.GetNearSelected(mygame.mypmx, mygame.Camera, e.Location) != -1) ret = Move.GetInstance();
            else if (mygame.vS.GetNearUsed(mygame.mypmx, mygame.Camera, e.Location) != -1) { mygame.vS.selectedVertexIndex = new int[] { mygame.vS.GetNearUsed(mygame.mypmx, mygame.Camera, e.Location) }; ret = Move.GetInstance(); }
            else ret = Select.GetInstance();
            mygame.drivingMode = ret;
        }

        public abstract DrawingMode Type();

        public abstract void MouseDowend(MyGame sender, MouseEventArgs e);
        public abstract void MouseMoved(MyGame sender, MouseEventArgs e);
        public abstract void MouseUpped(MyGame sender, MouseEventArgs e);
    }
}
