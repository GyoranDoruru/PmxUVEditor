using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyUVEditor.DrivingMode;
using System.Windows.Forms;
using SlimDX;
using MyUVEditor.Camera;

namespace MyUVEditor.DrivingMode
{
    class Remove : ChangeSelector
    {
        private static Remove instance = new Remove();
        private Remove() { }
        public static Remove GetInstance() { return instance; }

        public override DrawingMode Type() { return DrawingMode.REMOVE; }

        protected override bool Is2Add(int i, MyPMX pmx, VertexSelect vS, ICamera camera)
        {
            bool isUsed = pmx.IsUsedinMat[vS.SelectedMaterial, i];
            bool isSelected = camera.IsContainIt(vS.SelectPoly[0].Position, vS.SelectPoly[3].Position, pmx.VertexArray[i].Position);
            bool isContained = vS.selectedVertexIndex.Contains(i);

            return isContained && isUsed && !isSelected;
        }

    }

}
