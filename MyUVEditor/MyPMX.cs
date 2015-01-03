using System.Collections.Generic;
using PEPlugin;
using PEPlugin.Pmx;
using SlimDX;
using SlimDX.Direct3D9;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MyUVEditor
{
    public class MyPMX
    {
        IPEPluginHost host;
        public IPXPmx pmx;
        public CustomVertex.PositionOnly[] VertexArray;
        public int[] IndexArray;
        public bool[,] IsUsedinMat;
        public ExtendedMaterial[] Exmaterials;
        public AttributeRange[] AttributeRanges;
        MessageForm message = new MessageForm();

        public MyPMX(IPEPluginHost _host,MessageForm _message)
        {
            this.host = _host;
            this.message = _message;
            this.pmx = host.Connector.Pmx.GetCurrentState();
            this.SetMyPmx();
        }

        public void SetMyPmx()
        {
            int allfacecount = 0;
            foreach (IPXMaterial mat in pmx.Material) { allfacecount += mat.Faces.Count; }
            message.LabelReflesh("モデル読み込み");
            message.StatusBar.Value = 0;
            message.Visible = true;
            this.VertexArray = new CustomVertex.PositionOnly[pmx.Vertex.Count];
            this.IndexArray = new int[allfacecount * 3];
            this.Exmaterials = new ExtendedMaterial[pmx.Material.Count];
            this.AttributeRanges = new AttributeRange[pmx.Material.Count];
            this.IsUsedinMat = new bool[pmx.Material.Count, pmx.Vertex.Count];

            //VertexArray
            SetVertex();

            //IsUsedMat
            //IndexArray
            SetFaces(allfacecount);

            //AttributeRanges
            //Exmaterials
            SetMaterial();
        }

        /// <summary>
        /// 頂点の設定
        /// </summary>
        private void SetVertex()
        {
            message.LabelReflesh("モデル読み込み: 頂点");
            message.StatusBar.Maximum = pmx.Vertex.Count;
            for (int vertexidx = 0; vertexidx < pmx.Vertex.Count; vertexidx++)
            {
                IPXVertex v = pmx.Vertex[vertexidx];
                VertexArray[vertexidx] = new CustomVertex.PositionOnly(new SlimDX.Vector3(v.UV.U, v.UV.V, 0));
                message.StatusBar.Value++;
            }

        }
        /// <summary>
        /// 面頂点と材質-頂点使用フラグの設定
        /// </summary>
        /// <param name="allfacecount">全面頂点数</param>
        private void SetFaces(int allfacecount)
        {
            message.LabelReflesh("モデル読み込み: 面頂点");
            message.StatusBar.Maximum = allfacecount;
            message.StatusBar.Value = 0;

            Dictionary<IPXVertex, int> vHashIndex = new Dictionary<IPXVertex, int>();
            for (int i = 0; i < this.pmx.Vertex.Count; i++) vHashIndex.Add(pmx.Vertex[i], i);


            IsUsedinMat = new bool[this.pmx.Material.Count, this.VertexArray.Length];
            int index = 0;
            for (int matidx = 0; matidx < pmx.Material.Count; matidx++)
            {
                foreach (IPXFace f in pmx.Material[matidx].Faces)
                {
                    int vi = vHashIndex[f.Vertex1];
                    this.IndexArray[index] = vi;
                    this.IsUsedinMat[matidx, vi] = true;


                    vi = vHashIndex[f.Vertex2];
                    this.IndexArray[index + 1] = vi;
                    this.IsUsedinMat[matidx, vi] = true;

                    vi = vHashIndex[f.Vertex3];
                    this.IndexArray[index + 2] = vi;
                    this.IsUsedinMat[matidx, vi] = true;

                    index += 3;
                    message.StatusBar.Value++;
                }
            }

        }
        /// <summary>
        /// 材質設定
        /// </summary>
        private void SetMaterial()
        {           
            message.LabelReflesh("モデル読み込み: 材質");
            message.StatusBar.Value = 0;
            message.StatusBar.Maximum = pmx.Material.Count;
            int facestart = 0;
            for (int matidx = 0; matidx < this.pmx.Material.Count; matidx++)
            {
                IPXMaterial mat = this.pmx.Material[matidx];
                this.AttributeRanges[matidx] = new AttributeRange();
                this.AttributeRanges[matidx].AttribId = matidx;
                this.AttributeRanges[matidx].FaceCount = mat.Faces.Count;
                this.AttributeRanges[matidx].FaceStart = facestart;
                facestart += mat.Faces.Count;
                this.AttributeRanges[matidx].VertexCount = this.GetUsedVertexList(matidx).Count;
                this.AttributeRanges[matidx].VertexStart = pmx.Vertex.IndexOf(mat.Faces[0].Vertex1);

                this.Exmaterials[matidx] = new ExtendedMaterial();
                this.Exmaterials[matidx].TextureFileName = mat.Tex;
                Material tmpmat = new Material();
                tmpmat.Ambient = new Color4(mat.Diffuse.A, mat.Ambient.R, mat.Ambient.G, mat.Ambient.B);
                tmpmat.Diffuse = mat.Diffuse.ToColor4();
                tmpmat.Power = mat.Power;
                tmpmat.Specular = new Color4(mat.Specular.ToColor3());
                this.Exmaterials[matidx].MaterialD3D = tmpmat;
                message.StatusBar.Value++;
            }
        }
        /// <summary>
        /// 指定材質で使われている頂点リストの抽出
        /// </summary>
        /// <param name="matidx">指定材質番号</param>
        /// <returns></returns>
        private List<int> GetUsedVertexList(int matidx)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < VertexArray.Length; i++)
            {
                if (IsUsedinMat[matidx, i])
                {
                    result.Add(i);
                }
            }
            return result;
        }


 
        #region ボタンイベント用
        public void SendModel()
        {
            for (int i = 0; i < VertexArray.Length; i++)
            {
                pmx.Vertex[i].UV = new PEPlugin.SDX.V2(VertexArray[i].Position.X, VertexArray[i].Position.Y);
            }
            //更新
            host.Connector.Pmx.Update(pmx);
            host.Connector.Form.UpdateList(PEPlugin.Pmd.UpdateObject.Vertex);  // 重い場合は引数を変更して個別に更新
            host.Connector.View.PMDView.UpdateModel();         // Viewの更新が不要な場合はコメントアウト
            host.Connector.View.PMDView.UpdateView();
        }

        public void ReceiveModel()
        {
            this.pmx = host.Connector.Pmx.GetCurrentState();
            this.SetMyPmx();
            message.Visible = false;
        }        

        public int[] ReceiveSelected(int matidx)
        {
            List<int> tmpselected = new List<int>(host.Connector.View.PMDView.GetSelectedVertexIndices());
            List<int> resultselected = new List<int>();
            foreach (int i in tmpselected)
            {
                if (IsUsedinMat[matidx, i]) resultselected.Add(i);
            }
            resultselected.Sort();
            return resultselected.ToArray();
        }
        public void SendSelected(int[] sendArray)
        {
            host.Connector.View.PMDView.SetSelectedVertexIndices(sendArray);
            host.Connector.View.PMDView.UpdateView();
        }

        #endregion


        private StringBuilder IsUsedString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < IsUsedinMat.GetLength(0); i++)
            {
                string s = "";
                for (int j = 0; j < IsUsedinMat.GetLength(1); j++)
                {
                    s += this.IsUsedinMat[i, j] + ", ";
                }
                sb.AppendLine(s);
            }
            return sb;
        }
        /// <summary>デバッグログ出力用</summary>
        void StringOutDataStream(StringBuilder sb)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // CSVファイルとして保存
                    File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.GetEncoding("shift_jis"));
                }
            }
        }
    }

}