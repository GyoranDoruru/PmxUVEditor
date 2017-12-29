using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;

namespace MyUVEditor.DrivingMode
{
    class Move:Mode
    {
        private static Move instance=new Move();
        private Move() { }
        public static Move GetInstance() { return instance; }

        public override DrawingMode Type() { return DrawingMode.MOVE; }

        public override void MouseDowend(MyGame sender, MouseEventArgs e)
        {
            sender.form.PushUndo(new UnDoRedoRecord(sender.vS.selectedVertexIndex, sender.mypmx));
        }

        public override void MouseMoved(MyGame sender, MouseEventArgs e)
        {
            Vector3 vec = sender.Camera.ScreenToWorldVec(sender.Mouse.MouseAfterMoveP, 0)
                - sender.Camera.ScreenToWorldVec(sender.Mouse.MouseBeforeMoveP, 0);
            MoveMeshVertex(sender,vec);
        }

        public override void MouseUpped(MyGame sender, MouseEventArgs e)
        {
        }

        public void MoveMeshVertex(MyGame sender,Vector3 vec)
        {
            foreach (int i in sender.vS.selectedVertexIndex)
            {
                sender.mypmx.VertexArray[i].Position += vec;
            }
            sender.ResetMeshVertex();
            sender.ResetSelectedBuffer();
        }


        public void ButtonClicked(MyGame sender, Vector2 move)
        {
            if (move.Length() == 0) return;
            sender.form.PushUndo(new UnDoRedoRecord(sender.vS.selectedVertexIndex, sender.mypmx));
            foreach (int i in sender.vS.selectedVertexIndex)
            {
                sender.mypmx.VertexArray[i].Position.X += move.X;
                sender.mypmx.VertexArray[i].Position.Y += move.Y;
            }
            sender.ResetMeshVertex();
            sender.ResetSelectedBuffer();
        }
    }
}
