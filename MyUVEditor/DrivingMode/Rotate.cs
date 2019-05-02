using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;

namespace MyUVEditor.DrivingMode
{
    class Rotate:Mode
    {
        private static Rotate instance=new Rotate();
        private Rotate() { }
        public static Rotate GetInstance() { return instance; }

        public override DrawingMode Type() { return DrawingMode.ROTATE; }

        private Vector3[] savedPos;
        private Vector3 center;

        public override void MouseDowend(MyGame sender, MouseEventArgs e)
        {
            sender.form.PushUndo(new UnDoRedoRecord(sender.vS.selectedVertexIndex, sender.mypmx));
            this.savedPos = new Vector3[sender.mypmx.VertexArray.Length];
            foreach (int i in sender.vS.selectedVertexIndex) this.savedPos[i] = sender.mypmx.VertexArray[i].Position;
            center = sender.Camera.ScreenToWorldVec(sender.Mouse.MouseDowendP);
        }

        public override void MouseMoved(MyGame sender, MouseEventArgs e) { RotateMeshVertex(sender); }

        public override void MouseUpped(MyGame sender, MouseEventArgs e) { }

        private float GetAngle(MouseEvent mouse)
        {
            float k = 0.01f;
            int dpx = mouse.MouseAfterMoveP.X - mouse.MouseDowendP.X;
            int dpy = mouse.MouseAfterMoveP.Y - mouse.MouseDowendP.Y;
            double result = System.Math.Sign(dpx) * System.Math.Pow((dpx * dpx + dpy * dpy), 0.5) * k;
            if (KeyBoardEvent.GetInstance().IsShift) result *= 0.1;
            if (KeyBoardEvent.GetInstance().IsCtrl) result = ((int)(result * 4 / System.Math.PI)) * System.Math.PI / 4;
            return (float)result;
        }

        private void RotateMeshVertex(MyGame sender)
        {
            float angle = this.GetAngle(sender.Mouse);
            Matrix rotM = Matrix.Transformation(center, Quaternion.Identity, new Vector3(1,1,1), center, Quaternion.RotationAxis(Vector3.UnitZ, angle), Vector3.Zero);

            foreach (int i in sender.vS.selectedVertexIndex)
            {
                Vector4 newPos4 = Vector3.Transform(savedPos[i], rotM);
                sender.mypmx.VertexArray[i].Position.X = newPos4.X;
                sender.mypmx.VertexArray[i].Position.Y = newPos4.Y;
            }
            sender.ResetMeshVertex();
            sender.ResetSelectedBuffer();
        }


        public void ButtonClicked(MyGame sender, float angle, Vector2 center2)
        {
            if (angle == 0) return;
            sender.form.PushUndo(new UnDoRedoRecord(sender.vS.selectedVertexIndex, sender.mypmx));
            Vector3 center = new Vector3(center2, 0);
            Matrix rotM = Matrix.Transformation(center, Quaternion.Identity, new Vector3(1, 1, 1), center, Quaternion.RotationAxis(Vector3.UnitZ, angle), Vector3.Zero);
            foreach (int i in sender.vS.selectedVertexIndex)
            {
                Vector4 newPos4 = Vector3.Transform(sender.mypmx.VertexArray[i].Position, rotM);
                sender.mypmx.VertexArray[i].Position.X = newPos4.X;
                sender.mypmx.VertexArray[i].Position.Y = newPos4.Y;
            }
            sender.ResetMeshVertex();
            sender.ResetSelectedBuffer();
        }
    }
}
