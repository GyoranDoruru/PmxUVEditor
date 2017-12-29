using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using MyUVEditor.Camera;

namespace MyUVEditor.DrivingMode
{
    abstract class ChangeSelector:Mode
    {

        public override void MouseDowend(MyGame sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        public override void MouseMoved(MyGame sender, System.Windows.Forms.MouseEventArgs e)
        {
            Vector3 vec = sender.Camera.ScreenToWorldVec(e.Location, 0);
            sender.vS.ChangeSelectPoly(vec);
        }

        public override void MouseUpped(MyGame sender, System.Windows.Forms.MouseEventArgs e)
        {
            List<int> tmpselectedlist = new List<int>();
            MyPMX mypmx = sender.mypmx;
            VertexSelect vS = sender.vS;
            for (int i = 0; i < mypmx.VertexArray.Length; i++)
            {
                bool contained = Is2Add(i,mypmx, vS, sender.Camera);
                if (contained)
                {
                    tmpselectedlist.Add(i);
                }
            }
            vS.selectedVertexIndex = tmpselectedlist.ToArray();
            sender.ResetSelectedBuffer();
            sender.drivingMode = Nothing.GetInstance();
        }


        protected abstract bool Is2Add(int i,MyPMX pmx, VertexSelect vS,ICamera camera);

        public override DrawingMode Type() { return DrawingMode.SELECT; }


    }
}
