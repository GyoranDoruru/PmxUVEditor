using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;
using SlimDX.Direct3D9;

namespace MyUVEditor
{
    public class VertexSelect
    {
        public VertexSelect()
        {
            Color4 color = Color.White;
            color.Alpha = 0.6f;
            int c = color.ToArgb();
            // 各頂点を設定
            selectPoly[0] = new CustomVertex.PositionColored(new Vector3(0, 0, 0.01f), c);
            selectPoly[1] = new CustomVertex.PositionColored(new Vector3(0, 0, 0.01f), c);
            selectPoly[2] = new CustomVertex.PositionColored(new Vector3(0, 0, 0.01f), c);
            selectPoly[3] = new CustomVertex.PositionColored(new Vector3(0, 0, 0.01f), c);
        }

        public int SelectedMaterial { get; set; }
        private CustomVertex.PositionColored[] selectPoly = new CustomVertex.PositionColored[4];
        public CustomVertex.PositionColored[] SelectPoly { get { return this.selectPoly; } }

        private int nearUsedIndex = -1;
        public int NearUsedIndex { get { return this.nearUsedIndex; } }
        public bool IsNearUsed { get { return nearUsedIndex > 0; } }

        private CustomVertex.PositionOnly[] nearva = new CustomVertex.PositionOnly[1];
        private CustomVertex.PositionOnly[] selectedVs = new CustomVertex.PositionOnly[0];

        public int[] selectedVertexIndex = new int[0];

        public void PutSelectPoly(Vector3 vec)
        {
            vec.Z = 0.01f;
            selectPoly[0].Position = vec;
            selectPoly[1].Position = vec;
            selectPoly[2].Position = vec;
            selectPoly[3].Position = vec;
        }

        public void ChangeSelectPoly(Vector3 vec)
        {
            this.selectPoly[1].Position.X = vec.X;
            this.selectPoly[2].Position.Y = vec.Y;
            this.selectPoly[3].Position.X = vec.X;
            this.selectPoly[3].Position.Y = vec.Y;
        }

        public int GetNearSelected(MyPMX target, Camera camera, Point p)
        {
            for (int i = 0; i < this.selectedVertexIndex.Length; i++)
            {
                if (camera.PointIsNear(p, target.VertexArray[this.selectedVertexIndex[i]].Position))
                {
                    return this.selectedVertexIndex[i];
                }
            }
            return -1;
        }
        public int GetNearUsed(MyPMX target,Camera camera, Point p)
        {
            for (int i = 0; i < target.VertexArray.Length; i++)
            {
                bool used = target.IsUsedinMat[this.SelectedMaterial, i];
                if (used && camera.PointIsNear(p, target.VertexArray[i].Position))
                {
                    this.nearUsedIndex = i;
                    return i;
                }
            }
            this.nearUsedIndex = -1;
            return -1;
        }

    }
}
