using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;

namespace MyUVEditor.DrivingMode
{
    class Scale:Mode
    {
        private static Scale instance=new Scale();
        private Scale() { }
        public static Scale GetInstance() { return instance; }
        public override DrawingMode Type() { return DrawingMode.SCALE; }
        private Vector3[] savedPos;
        private Vector3 center;

        public override void MouseDowend(MyGame sender, MouseEventArgs e)
        {
            sender.form.PushUndo(new UnDoRedoRecord(sender.vS.selectedVertexIndex, sender.mypmx));
            this.savedPos = new Vector3[sender.mypmx.VertexArray.Length];
            foreach (int i in sender.vS.selectedVertexIndex) this.savedPos[i] = sender.mypmx.VertexArray[i].Position;
            center = Camera.Screen2World(sender.Device, sender.Mouse.MouseDowendP);
        }

        public override void MouseMoved(MyGame sender, MouseEventArgs e) { ScaleMeshVertex(sender); }

        public override void MouseUpped(MyGame sender, MouseEventArgs e) { }


        private Vector2 GetRatio(MouseEvent mouse)
        {
            float k = 0.01f;
            int dpx = mouse.MouseAfterMoveP.X - mouse.MouseDowendP.X;
            int dpy = mouse.MouseAfterMoveP.Y - mouse.MouseDowendP.Y;
            bool strongX = Math.Abs(dpx) > Math.Abs(dpy);

            float dx, dy;
            if (KeyBoardEvent.GetInstance().IsShift)
            {
                dx = strongX ? dpx * k : dpy * k;
                dy = dx;
            }
            else if (KeyBoardEvent.GetInstance().IsCtrl)
            {
                dx = strongX ? dpx * k : 0;
                dy = strongX ? 0 : dpy * k;
            }

            else { dx = dpx * k; dy = dpy * k; }
            return new Vector2(1.0f+dx,1.0f+dy);
        }

        private void ScaleMeshVertex(MyGame sender)
        {
            Vector2 ratio = this.GetRatio(sender.Mouse);


            foreach (int i in sender.vS.selectedVertexIndex)
            {
                sender.mypmx.VertexArray[i].Position.X = (savedPos[i].X - center.X) * ratio.X + center.X;
                sender.mypmx.VertexArray[i].Position.Y = (savedPos[i].Y - center.Y) * ratio.Y + center.Y;
            }
            sender.ResetMeshVertex();
            sender.ResetSelectedBuffer();
        }

        public void ButtonClicked(MyGame sender, Vector2 ratio,Vector2 center)
        {
            if (ratio.X == 1 && ratio.Y == 1) return;
            sender.form.PushUndo(new UnDoRedoRecord(sender.vS.selectedVertexIndex, sender.mypmx));
            foreach (int i in sender.vS.selectedVertexIndex)
            {
                sender.mypmx.VertexArray[i].Position.X = (sender.mypmx.VertexArray[i].Position.X - center.X) * ratio.X + center.X;
                sender.mypmx.VertexArray[i].Position.Y = (sender.mypmx.VertexArray[i].Position.Y - center.Y) * ratio.Y + center.Y;
            }
            sender.ResetMeshVertex();
            sender.ResetSelectedBuffer();
        }
    }
}
