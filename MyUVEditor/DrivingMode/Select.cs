using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using MyUVEditor.Camera;

namespace MyUVEditor.DrivingMode
{
    class Select : ChangeSelector
    {
        private static Select instance = new Select();
        private Select() { }
        public static Select GetInstance() { return instance; }



        public override DrawingMode Type() { return DrawingMode.SELECT; }


        protected override bool Is2Add(int i,MyPMX pmx, VertexSelect vS, ICamera camera)
        {
            bool isUsed = pmx.IsUsedinMat[vS.SelectedMaterial, i];
            bool isSelected = camera.IsContainIt(vS.SelectPoly[0].Position, vS.SelectPoly[3].Position, pmx.VertexArray[i].Position);

            return isUsed&&isSelected;
        }
    }
}
