using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;

namespace MyUVEditor
{
    public partial class MyGame
    {
        public MyPMX mypmx;
        private Texture[] textures = null;
        private TextureInfo[] textureinfoes = null;
        private VertexBuffer texvertex = null;
        private VertexBuffer selectedbuffer = null;
        public  VertexSelect vS;
        private MouseEvent mouse = new MouseEvent();
        public MouseEvent Mouse { get { return mouse; } }
        public DrivingMode.Mode drivingMode = DrivingMode.Nothing.GetInstance();

        private bool LoadPMXMesh()
        {
            this.mesh = null;
            try
            {
                this.mypmx = new MyPMX(this.host,this.message);
                this.ConvertMyPMXtoMesh();
            }
            catch (Direct3D9Exception ex)
            {
                MessageBox.Show("モデルが読み込まれてないのかも？");
                this.Dispose();
                throw ex;
            }

            this.SetTextures();
            return true;
        }

        private void ConvertMyPMXtoMesh()
        {
            message.LabelReflesh("メッシュ変換中");
            message.StatusBar.Value = 0;
            message.StatusBar.Maximum = 6;
            this.mesh = null;
            this.mesh = new Mesh(device, this.mypmx.IndexArray.Length / 3, this.mypmx.VertexArray.Length, MeshFlags.Use32Bit | MeshFlags.Managed, CustomVertex.PositionOnly.FVF);
            message.StatusBar.Value = 1;
            using (DataStream data = this.mesh.LockIndexBuffer(LockFlags.None))
            {
                data.WriteRange(this.mypmx.IndexArray);
                this.mesh.UnlockIndexBuffer();
            }
            message.StatusBar.Value = 2;

            using (DataStream data = this.mesh.LockVertexBuffer(LockFlags.None))
            {
                // 頂点データを頂点バッファにコピーします
                data.WriteRange(this.mypmx.VertexArray);

                // 頂点バッファのロックを解除します
                this.mesh.UnlockVertexBuffer();
            }
            message.StatusBar.Value = 3;

            mesh.SetAttributeTable(this.mypmx.AttributeRanges);
            message.StatusBar.Value = 4;
            using (DataStream data = this.mesh.LockAttributeBuffer(LockFlags.None))
            {
                for (int i = 0; i < this.mypmx.AttributeRanges.Length; i++)
                {
                    for (int j = 0; j < this.mypmx.AttributeRanges[i].FaceCount; j++)
                    {
                        data.Write<int>(i);
                    }
                }
                this.mesh.UnlockAttributeBuffer();
            }
            message.StatusBar.Value = 5;

            this.mesh.SetMaterials(this.mypmx.Exmaterials);
            message.StatusBar.Value = 6;

        }

        private void ConvertMeshtoMesh2D(Mesh old)
        {
            CustomVertex.PositionOnly[] oldVertexArray = new CustomVertex.PositionOnly[old.VertexCount];
            using (DataStream data = old.LockVertexBuffer(LockFlags.None))
            {
                for (int i = 0; i < old.VertexCount; i++)
                {
                    float[] vertexdata = new float[8];
                    for (int j = 0; j < 8; j++)
                    {
                        vertexdata[j] = data.Read<float>();
                    }
                    oldVertexArray[i] = new CustomVertex.PositionOnly(new Vector3(vertexdata[6], vertexdata[7], 0));
                }

                old.UnlockVertexBuffer();
            }

            Int16[] oldIndexArray = new Int16[old.FaceCount * 3];
            using (DataStream data = old.LockIndexBuffer(LockFlags.None))
            {
                for (int i = 0; i < old.FaceCount * 3; i++)
                {
                    oldIndexArray[i] = data.Read<Int16>();
                }
                old.UnlockIndexBuffer();
            }

            List<int> oldAttributeArray = new List<int>();
            using (DataStream data = old.LockAttributeBuffer(LockFlags.None))
            {
                while (data.Position<data.Length)
                {
                    oldAttributeArray.Add(data.Read<int>());
                }
                old.UnlockAttributeBuffer();
            }
            this.mesh = new Mesh(device, old.FaceCount, old.VertexCount, MeshFlags.Managed, VertexFormat.Position);

            using (DataStream data = this.mesh.LockVertexBuffer(LockFlags.None))
            {
                data.WriteRange(oldVertexArray);
                mesh.UnlockVertexBuffer();
            }

            using (DataStream data = this.mesh.LockIndexBuffer(LockFlags.None))
            {
                data.WriteRange(oldIndexArray);
                mesh.UnlockIndexBuffer();
            }

            using (DataStream data = this.mesh.LockAttributeBuffer(LockFlags.None))
            {
                for (int i = 0; i < oldAttributeArray.Count; i++)
                {
                    data.Write(oldAttributeArray[i]);
                }
                mesh.UnlockAttributeBuffer();
            }
           // AttributeRange[] ars = old.GetAttributeTable();
            //this.mesh.SetAttributeTable(ars);
            this.mesh.SetMaterials(old.GetMaterials());
            this.mesh.SetAdjacency(old.GetAdjacency());

            //this.mypmx = new MyPMX(oldVertexArray,oldIndexArray,old.GetMaterials(),old.GetAttributeTable());
        }

        /// <summary>テクスチャ読み込み</summary>
        private void SetTextures()
        {
            form.SelectedMaterial.Items.Clear();
            if (this.mypmx.Exmaterials.Length > 0)
            {
                message.LabelReflesh("テクスチャ画像読み込み");
                message.StatusBar.Value = 0;
                message.StatusBar.Maximum = mypmx.Exmaterials.Length;
                this.textures = new Texture[this.mypmx.Exmaterials.Length];
                this.textureinfoes = new TextureInfo[this.mypmx.Exmaterials.Length];
                System.Environment.CurrentDirectory = Path.GetDirectoryName(this.mypmx.pmx.FilePath);
                for (int i = 0; i < this.mypmx.Exmaterials.Length; i++)
                {
                    this.textures[i] = null;
                    string fName = this.mypmx.Exmaterials[i].TextureFileName;
                    if (fName != null &&
                        fName.Length >= 1)
                    {
                        try
                        {
                            this.textures[i] = Texture.FromFile(this.device, fName);
                        }
                        catch (Direct3D9Exception) { }
                    }
                    this.textureinfoes[i] = new TextureInfo(fName);
                    form.AddMaterial(this.mypmx.pmx.Material[i].Name);
                    message.StatusBar.Value++;
                }
            }
        }


        private void RenderMesh()
        {
            for (int i = 0; i < this.mypmx.Exmaterials.Length; i++)
            {
                //テクスチャの描画
                this.device.SetTexture(0, this.textures[i]);

                //マテリアルのセット
                this.device.Material = this.mypmx.Exmaterials[i].MaterialD3D;

                //描画
                this.mesh.DrawSubset(i);
            }
        }

        private void CreateTexPoly()
        {
            this.texvertex = new VertexBuffer(device, CustomVertex.PositionTextured.Stride * 4, Usage.None, CustomVertex.PositionTextured.FVF, Pool.Managed);

            // ４点の情報を格納するためのメモリを確保
            CustomVertex.PositionTextured[] vertices = new CustomVertex.PositionTextured[4];

            // 各頂点を設定
            vertices[0] = new CustomVertex.PositionTextured(new Vector3(0.0f, 1.0f, -0.01f), 0.0f, 1.0f);
            vertices[1] = new CustomVertex.PositionTextured(new Vector3(1.0f, 1.0f, -0.01f), 1.0f, 1.0f);
            vertices[2] = new CustomVertex.PositionTextured(new Vector3(0.0f, 0.0f, -0.01f), 0.0f, 0.0f);
            vertices[3] = new CustomVertex.PositionTextured(new Vector3(1.0f, 0.0f, -0.01f), 1.0f, 0.0f);

            // 頂点バッファをロックする
            using (DataStream data = this.texvertex.Lock(0, 0, LockFlags.None))
            {
                // 頂点データを頂点バッファにコピーします
                data.WriteRange(vertices);

                // 頂点バッファをロック解除します
                this.texvertex.Unlock();
            }
        }

        private void CreateSelectPoly()
        {
            message.LabelReflesh("選択用矩形オブジェクト生成");
            message.StatusBar.Value = 0;
            this.vS = new VertexSelect();
            message.StatusBar.Value = message.StatusBar.Maximum;
        }

        private void SetBuffersTextures()
        {
            try
            {
                LoadPMXMesh();
                //mesh = MakeOriginalMesh();
            }
            catch (Direct3D9Exception ex) { MessageBox.Show(ex.ToString(), "エラー"); }

            //
            this.CreateTexPoly();
        }


        #region マウスイベント

        private void CreateMouseEvent(Control control)
        {
            control.MouseMove += new MouseEventHandler(this.form_MouseMove);
            control.MouseDown += new MouseEventHandler(this.form_MouseDown);
            control.MouseWheel += new MouseEventHandler(this.form_MouseWheel);
            control.MouseUp += new MouseEventHandler(this.form_MouseUp);
        }

        public void form_MouseDown(object sender, MouseEventArgs e)
        {
            this.mouse.MouseDown(e);
            drivingMode.GetDrivingMode(this, e);
            drivingMode.MouseDowend(this, e);
            Vector3 vec = Camera.Screen2World(this.device, e.Location);
            vS.PutSelectPoly(vec);
            this.Render();
        }

        public void form_MouseMove(object sender, MouseEventArgs e)
        {
            this.mouse.MouseMove(e);
            this.drivingMode.MouseMoved(this, e);
            this.form.UVLocation = Camera.Screen2World(this.device,e.Location);
            this.Render();
        }

        public void form_MouseUp(object sender, MouseEventArgs e)
        {
            this.mouse.MouseUp(e);
            if (this.drivingMode.Type() != DrawingMode.MOVE)
            {
                    this.drivingMode.MouseUpped(this, e);
            }
            //this._drawingMode = DrawingMode.MOVE;
            //this.mouse.MouseMovedP = e.Location;
            this.drivingMode = DrivingMode.Nothing.GetInstance();
            this.Render();
        }




        public void form_MouseWheel(object sender, MouseEventArgs e)
        {
            this.camera.ScaleChange(e.Delta);
            this.Render();
        }

        #endregion


        /// <summary>メッシュの頂点情報をリセット</summary>
        public void ResetMeshVertex()
        {
            using (DataStream data = mesh.LockVertexBuffer(LockFlags.None))
            {
                // 頂点データを頂点バッファにコピーします
                data.WriteRange(mypmx.VertexArray);

                // 頂点バッファのロックを解除します
                mesh.UnlockVertexBuffer();
            }
        }

        /// <summary>選択頂点のバッファをリセット</summary>
        public void ResetSelectedBuffer()
        {
            if (this.vS.selectedVertexIndex.Length < 1) return;
            this.selectedbuffer = new VertexBuffer(device, CustomVertex.PositionOnly.Stride * vS.selectedVertexIndex.Length, Usage.None, CustomVertex.PositionOnly.FVF, Pool.Managed);
            using (DataStream data = this.selectedbuffer.Lock(0, 0, LockFlags.None))
            {
                for (int i = 0; i < vS.selectedVertexIndex.Length; i++)
                {
                    data.Write<CustomVertex.PositionOnly>(mypmx.VertexArray[vS.selectedVertexIndex[i]]);
                }
                this.selectedbuffer.Unlock();
            }

        }


        /// <summary>頂点配列の座標を動かし各頂点バッファ更新</summary><param name="vec"></param>
        void MooveMeshVertex(Vector3 vec)
        {
            foreach (int i in this.vS.selectedVertexIndex)
            {
                mypmx.VertexArray[i].Position += vec;
            }
            ResetMeshVertex();
            ResetSelectedBuffer();
        }




        #region デバッグ
        private StringBuilder StringAttribute()
        {
            AttributeRange[] tmpar = mesh.GetAttributeTable();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ID, FaceCount, FaceStart, VertexCount, VertexStart\r\n");
            foreach (AttributeRange ar in tmpar)
            {
                sb.Append(ar.AttribId + ", ");
                sb.Append(ar.FaceCount + ", ");
                sb.Append(ar.FaceStart + ", ");
                sb.Append(ar.VertexCount + ", ");
                sb.Append(ar.VertexStart + "\r\n");
            }
            return sb;
        }

        private StringBuilder MeshVertexBufferStream()
        {
            StringBuilder sb = new StringBuilder();
            using (DataStream data = mesh.LockVertexBuffer(LockFlags.None))
            {
                int counter = 0;
                while (counter<100)//data.Position < data.Length)
                {
                    float s = data.Read<float>();
                    sb.AppendLine(s.ToString());
                    counter++;
                }
                mesh.UnlockVertexBuffer();
            }
            return sb;
        }

        private StringBuilder MeshAttributeBufferStream()
        {
            StringBuilder sb = new StringBuilder();
            using (DataStream data = mesh.LockAttributeBuffer(LockFlags.None))
            {
                int counter = 0;
                while (data.Position<data.Length)//data.Position < data.Length)
                {
                    int s = data.Read<int>();
                    sb.Append(s);
                    counter++;
                }
                MessageBox.Show(counter.ToString());
                mesh.UnlockAttributeBuffer();
            }
            return sb;
        }

        private StringBuilder IndexBufferStream(IndexBuffer ib)
        {
            StringBuilder sb = new StringBuilder();
            using (DataStream data = ib.Lock(0, 0, LockFlags.None))
            {
                while (data.Position < data.Length)
                {
                    int b = data.Read<int>();
                    sb.Append(b+", ");
                }
                ib.Unlock();
            }
            return sb;
        }

        private StringBuilder IsUsedBufferStream()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mypmx.pmx.Material.Count; i++)
            {
                sb.AppendLine(i.ToString());
                for (int j = 0; j < mypmx.VertexArray.Length; j++)
                {
                    sb.AppendLine(mypmx.IsUsedinMat[i, j].ToString());
                }
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
        #endregion
    }

}
