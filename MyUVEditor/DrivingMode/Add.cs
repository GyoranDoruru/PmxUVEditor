using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;

namespace MyUVEditor.DrivingMode
{
    class Add : ChangeSelector
    {
        private static Add instance = new Add();
        private Add() { }
        public static Add GetInstance() { return instance; }


        public override DrawingMode Type() { return DrawingMode.ADD; }

        protected override bool Is2Add(int i, MyPMX pmx, VertexSelect vS, Camera camera)
        {
            bool isUsed = pmx.IsUsedinMat[vS.SelectedMaterial, i];
            bool isSelected = camera.IsContainIt(vS.SelectPoly[0].Position, vS.SelectPoly[3].Position, pmx.VertexArray[i].Position);
            bool isContained = vS.selectedVertexIndex.Contains(i);

            return isUsed && (isContained || isSelected);
        }
    }
}
