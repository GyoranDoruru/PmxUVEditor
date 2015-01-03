using System;
using System.Drawing;
using System.Windows.Forms;
using PEPlugin;
using SlimDX;
using SlimDX.Direct3D9;
using System.Collections.Generic;

namespace MyUVEditor
{
    public partial class  MyGame : IDisposable
    {
        /// <summary>フォーム</summary>
        public Form1 form;
        /// <summary>進捗ウィンドウ</summary>
        public MessageForm message = new MessageForm();
        /// <summary>Direct3Dオブジェクト</summary>
        private Direct3D direct3D;
        /// <summary>Direct3Dデバイス</summary>
        private Device device;
        public Device Device { get { return this.device; } }
        /// <summary>PresentParameters</summary>
        private PresentParameters pp;

        /// <summary>メッシュ</summary>
        private Mesh mesh;

        /// <summary>視点関連</summary>
        private Camera camera;
        public Camera Camera { get { return this.camera; } }

        private IPEPluginHost _host;
        /// <summary>デバイスロスト判定用</summary>
        private bool deviceLost = false;


        /// <summary>UVモーフ関連</summary>
        private MakeUVMorph uvmorphes; 

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="host"></param>
        /// <param name="ver"></param>
        public MyGame(IPEPluginHost host, string ver)
        {
            this._host = host;
            this.form = new Form1(ver);
            this.uvmorphes = new MakeUVMorph(host, this.form);
        }

        /// <summary>
        /// 初期化とループ開始
        /// </summary>
        public void Run()
        {
            //メッセージフォームを作成
            message.Show();
            message.StatusText = "フォーム作成中";

            #region 基本イベント追加
            message.LabelReflesh("イベント追加中");

            form.ResizeBegin += (o, args) =>
            {
                this.ResourceDispose();
            };

            form.ResizeEnd += (o, args) =>
            {
                this.camera.SetViewPortSize(form);
                pp.BackBufferWidth = form.ViewPanel.Width;
                pp.BackBufferHeight = form.ViewPanel.Height;
                device.Reset(pp);
                Viewport vp = device.Viewport;
                vp.Height = form.ViewPanel.Height;
                vp.Width = form.ViewPanel.Width;
                device.Viewport = vp;
                this.SetTextures();
                this.OnResourceLoad();
                this.Render();
            };

            form.Closed += (o, args) => { this.Dispose(); };

            form.SelectedMaterial.SelectedIndexChanged += (o, args) =>
            {
                this.vS.SelectedMaterial = this.form.SelectedMaterial.SelectedIndex;
                if (this.form.TabControll.SelectedIndex == 0) this.vS.selectedVertexIndex = new int[0];
                this.Render();
            };

            form.UnDo.Click += (o, args) =>
            {
                this.form.Undo(this.mypmx);
                ResetMeshVertex();
                ResetSelectedBuffer();
                this.Render();
            };
            form.ReDo.Click += (o, args) =>
            {
                this.form.Redo(this.mypmx);
                ResetMeshVertex();
                ResetSelectedBuffer();
                this.Render();
            };
            form.SelectAll.Click += (o, args) =>
            {
                if (this.form.TabControll.SelectedIndex == 1) return;
                List<int> tmpselectedlist = new List<int>();
                for (int i = 0; i < mypmx.VertexArray.Length; i++)
                {
                    bool contained = mypmx.IsUsedinMat[vS.SelectedMaterial, i];
                    if (contained)
                    {
                        tmpselectedlist.Add(i);
                    }
                }
                vS.selectedVertexIndex = tmpselectedlist.ToArray();
                ResetSelectedBuffer();
                this.Render();
                };

            CreateMouseEvent(form.ViewPanel);
            KeyBoardEvent.GetInstance().CreateInputEvent(form.ViewPanel);

            form.ReceiveSelectedButton.Click += (o, args) =>
            {
                this.vS.selectedVertexIndex = this.mypmx.ReceiveSelected(vS.SelectedMaterial);
                ResetSelectedBuffer();
                this.Render();
            };

            form.SendSelectedButton.Click += (o, args) =>
            {
                this.mypmx.SendSelected(this.vS.selectedVertexIndex);
            };

            form.ReceiveModelButton.Click += (o, args) =>
            {
                this.ResourceDispose();
                this.mesh.Dispose();
                this.form.SelectedMaterial.Items.Clear();
                this.mypmx.ReceiveModel();
                this.ConvertMyPMXtoMesh();
                this.SetTextures();
                this.vS.selectedVertexIndex = new int[0];
                ResetSelectedBuffer();
                this.uvmorphes.ReSetUVList();
                this.Render();

            };

            form.SendModelButton.Click += (o, args) =>
            {
                this.mypmx.SendModel();
            };

            form.MoveButton.Click += (o, args) => { DrivingMode.Move.GetInstance().ButtonClicked(this, this.form.MoveValue); };
            form.ScaleButton.Click += (o, args) => { DrivingMode.Scale.GetInstance().ButtonClicked(this, this.form.ScaleValue, this.form.CenterValue); };
            form.RotButton.Click += (o, args) => { DrivingMode.Rotate.GetInstance().ButtonClicked(this,this.form.RotValue,this.form.CenterValue);};

            form.ReadMorph.Click += (o, args) =>
            {
                this.uvmorphes.ReadMorph(this.mypmx,this.vS);
                this.form.MorphSlider.Value = this.form.MorphSlider.Maximum;
                ResetMeshVertex();
                ResetSelectedBuffer();
                Render();
            };
            form.SetMorph.Click += (o, args) =>
            {
                this.uvmorphes.MakeMorph(this.mypmx,this.vS);
                this.form.MorphSlider.Value = this.form.MorphSlider.Maximum;
                ResetMeshVertex();
                ResetSelectedBuffer();
                Render();
            };
            form.AddMorph.Click += (o, args) => { this.uvmorphes.AddMorph(); };
            form.MorphSlider.Scroll += (o, args) =>
            {
                float ratio = ((float)form.MorphSlider.Value) / form.MorphSlider.Maximum;
                this.uvmorphes.MoveMorph(mypmx, ratio);
                ResetMeshVertex();
                ResetSelectedBuffer();
                Render();
            };
            # endregion

            message.LabelReflesh("DirectX関連の設定中");

            //Direct3Dパラメータの設定
            pp = new PresentParameters()
            {
                BackBufferFormat = Format.X8R8G8B8,
                BackBufferCount = 1,
                BackBufferWidth = form.ViewPanel.Width,
                BackBufferHeight = form.ViewPanel.Height,
                Multisample = MultisampleType.None,
                SwapEffect = SwapEffect.Discard,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = Format.D16,
                PresentFlags = PresentFlags.DiscardDepthStencil,
                PresentationInterval = PresentInterval.Default,
                Windowed = true,
                DeviceWindowHandle = form.ViewPanel.Handle
            };

            //Direct3Dオブジェクト・デバイスを作成
            direct3D = new Direct3D();
            try
            {
                device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, pp);
                this.form.SetDriveState("松");
            }
            catch (Direct3D9Exception ex1)
            {
                try
                {
                    device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.SoftwareVertexProcessing, pp);
                    this.form.SetDriveState("竹\r\n"+ex1.Source+"\r\n"+ex1.Message);
                }
                catch (Direct3D9Exception ex2)
                {
                    try
                    {
                        device = new Device(direct3D, 0, DeviceType.Reference, form.Handle, CreateFlags.SoftwareVertexProcessing, pp);
                        this.form.SetDriveState("梅\r\n" + ex2.Source + "\r\n" + ex2.Message);
                    }
                    catch (Direct3D9Exception ex3)
                    {
                        MessageBox.Show(ex3.Message +"\r\nデバイスの作成に失敗しました。", "DirectX Initialization failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        form.Close();
                    }
                }
            }

            this.camera = new Camera(this.device);
            //高さと幅設定
            this.camera.SetViewPortSize(form);

            //Xファイルからmeshとtextures登録
            this.SetBuffersTextures();
            this.CreateSelectPoly();

            message.LabelReflesh("その他設定中");
            message.StatusBar.Value = 0;
            //初期化
            OnResourceLoad();
            message.StatusBar.Value = message.StatusBar.Maximum;

            message.LabelReflesh("初レンダリング開始");
            //Application.Run(form);
            this.Render();
            message.Visible = false;
            form.Show();
        }

        /// <summary>
        /// 初期化・リセット復帰後に呼ばれる
        /// </summary>
        private void OnResourceLoad()
        {
            //ライト設定
            device.SetRenderState(RenderState.Lighting, true);
            device.SetLight(0, new Light()
            {
                Type = LightType.Directional,
                Diffuse = Color.White,
                Ambient = Color.GhostWhite,
                Direction = new Vector3(0.0f, -1.0f, 0.0f)
            });
            device.EnableLight(0, true);

            //射影変換
            device.SetTransform(TransformState.Projection, this.camera.TransformProjection(1));
            //ビュー
            device.SetTransform(TransformState.View, camera.TransformView());

            //マテリアル設定
            //device.Material = new Material() { Diffuse = new Color4(Color.GhostWhite) };

            //カリング無視
            this.device.SetRenderState(RenderState.CullMode, Cull.None);

            //アルファブレンド
            this.device.SetRenderState(RenderState.SourceBlend , Blend.SourceAlpha);
            this.device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            device.SetRenderState(RenderState.AlphaBlendEnable, true);
        }

        /// <summary>
        /// 描画
        /// </summary>
        public void Render()
        {
            message.LabelReflesh(drivingMode.Type().ToString());
            //デバイスロスト時の対応
            if (deviceLost)
            {
                message.LabelReflesh("デバイスロスト");
                if (device.TestCooperativeLevel() == ResultCode.DeviceNotReset)
                {
                    device.Reset(pp);
                    deviceLost = false;
                    OnResourceLoad();
                    Render();
                }
                else
                {
                    Render();
                }
            }

            try
            {
                if (KeyBoardEvent.GetInstance().OnKeys[(int)Keys.Enter]) { this.camera.ResetCamera(this.device); MessageBox.Show("called"); }
                device.SetTransform(TransformState.View, this.camera.TransformView());
                device.SetTransform(TransformState.Projection, this.camera.TransformProjection(this.textureinfoes[vS.SelectedMaterial].Aspect));

                //描画を開始
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color4(0.3f, 0.3f, 0.3f), 1.0f, 0);
                device.BeginScene();

                //Xファイル
                device.Material = new Material()
                {
                    Diffuse = new Color4(Color.White),
                    Ambient = new Color4(Color.Black)
                };
                device.SetRenderState(RenderState.Lighting, true);
                device.SetRenderState(RenderState.FillMode, FillMode.Wireframe);
                 mesh.DrawSubset(vS.SelectedMaterial);

                //背景表示
                device.Material = new Material()
                {
                    Ambient = new Color4(Color.White)
                };
                device.SetTexture(0, textures[vS.SelectedMaterial]);
                device.SetRenderState(RenderState.FillMode, FillMode.Solid);
                device.SetStreamSource(0, texvertex, 0, CustomVertex.PositionTextured.Stride);
                device.VertexFormat = CustomVertex.PositionTextured.FVF;
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

                
                //頂点表示
                device.Material = new Material()
                {
                    Diffuse = new Color4(Color.Black),
                    Ambient = new Color4(Color.Black)
                };
                device.SetRenderState(RenderState.FillMode, FillMode.Point);
                device.SetRenderState(RenderState.PointSpriteEnable, true);
                device.SetRenderState(RenderState.PointSize, 6f);
                device.SetTexture(0, null);
                mesh.DrawSubset(this.vS.SelectedMaterial);

                //選択頂点表示
                if (vS.selectedVertexIndex.Length > 0)
                {
                    device.Material = new Material()
                    {
                        Diffuse = new Color4(Color.Red),
                        Ambient = new Color4(Color.Red)
                    };
                    device.SetRenderState(RenderState.FillMode, FillMode.Solid);
                    device.SetRenderState(RenderState.PointSpriteEnable, true);
                    device.SetStreamSource(0, selectedbuffer, 0, CustomVertex.PositionOnly.Stride);
                    device.VertexFormat = CustomVertex.PositionOnly.FVF;
                    device.DrawPrimitives(PrimitiveType.PointList,
                        0,
                        vS.selectedVertexIndex.Length);
                }

                //選択領域表示
                if ((this.drivingMode.Type() ==DrawingMode.ADD)||(this.drivingMode.Type() ==DrawingMode.REMOVE)||(this.drivingMode.Type() ==DrawingMode.SELECT))
                {
                    device.Material = new Material()
                    {
                        Diffuse = new Color4(0.2f, 1.0f, 1.0f, 1.0f),
                        Ambient = new Color4(0.2f, 1.0f, 1.0f, 1.0f)
                    };
                    device.SetRenderState(RenderState.FillMode, FillMode.Solid);
                    device.VertexFormat = CustomVertex.PositionColored.FVF;
                    device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, this.vS.SelectPoly);
                }

                if (this.vS.IsNearUsed)
                {
                    CustomVertex.PositionOnly[] nearva = { mypmx.VertexArray[this.vS.NearUsedIndex] };
                    device.Material = new Material() { Diffuse = Color.Yellow, Ambient = Color.Yellow };
                    device.DrawUserPrimitives(PrimitiveType.PointList, 1, nearva);                        
                }
                device.EndScene();
                device.Present();
            }

            catch (Direct3D9Exception e)
            {
                //デバイスロストかどうか判定
                if (e.ResultCode == ResultCode.DeviceLost)
                {
                    deviceLost = true;
                    Render();
                }
                else
                {
                    MessageBox.Show(e.Source);
                    throw;
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        public void Dispose()
        {
            this.ResourceDispose();
            if (mesh != null) mesh.Dispose();
            if (texvertex != null) texvertex.Dispose();
            device.Dispose();
            if (form != null)form.Dispose();
            if (message != null)message.Dispose();
        }

        public void ResourceDispose()
        {
            if (textures != null)
            {
                for (int i = 0; i < textures.Length; i++)
                {
                    if (textures[i] != null) { textures[i].Dispose(); }
                }
            }
        }
    }
}