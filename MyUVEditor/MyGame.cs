using System;
using System.Drawing;
using System.Windows.Forms;
using PEPlugin;
using SlimDX;
using SlimDX.Direct3D9;
using System.Collections.Generic;

namespace MyUVEditor
{
    public partial class MyGame : IDisposable
    {
        /// <summary>フォーム</summary>
        public Form1 form;
        /// <summary>モデル表示用フォーム</summary>
        public DXView subform;
        /// <summary>進捗ウィンドウ</summary>
        public MessageForm message = new MessageForm();
        /// <summary>Direct3Dオブジェクト</summary>
        private Direct3D direct3D;
        /// <summary>Direct3Dデバイス</summary>
        private Device device;
        public Device Device { get { return this.device; } }
        /// <summary>PresentParameters</summary>
        private PresentParameters[] pps;

        //SwapChain mainSwap;
        //SwapChain subSwap;
        /// <summary>メッシュ</summary>
        private Mesh mesh;

        /// <summary>視点関連</summary>
        private Camera camera;
        public Camera Camera { get { return this.camera; } }

        private IPEPluginHost host;
        /// <summary>デバイスロスト判定用</summary>
        private bool deviceLost = false;


        /// <summary>UVモーフ関連</summary>
        private MakeUVMorph uvmorphes;

        private MaterialManager mmanage;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="host"></param>
        /// <param name="ver"></param>
        public MyGame(IPEPluginHost _host, string ver)
        {
            this.host = _host;
            this.form = new Form1(ver);
            this.subform = new DXView();
            this.uvmorphes = new MakeUVMorph(_host.Connector, this.form);
            this.mmanage = new MaterialManager();
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
                pps[0].BackBufferWidth = form.ViewPanel.Width;
                pps[0].BackBufferHeight = form.ViewPanel.Height;
                Viewport vp = device.Viewport;
                vp.Height = form.ViewPanel.Height;
                vp.Width = form.ViewPanel.Width;
                device.Viewport = vp;
                device.Reset(pps);
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
                this.vS.selectedVertexIndex = this.mypmx.ReceiveSelected(host.Connector, vS.SelectedMaterial);
                ResetSelectedBuffer();
                this.Render();
            };

            form.SendSelectedButton.Click += (o, args) =>
            {
                this.mypmx.SendSelected(host.Connector, this.vS.selectedVertexIndex);
            };

            form.ReceiveModelButton.Click += (o, args) =>
            {
                this.ResourceDispose();
                this.mesh.Dispose();
                this.form.SelectedMaterial.Items.Clear();
                this.mypmx.ReceiveModel(host.Connector);
                this.ConvertMyPMXtoMesh();
                this.SetTextures();
                this.vS.selectedVertexIndex = new int[0];
                ResetSelectedBuffer();
                this.uvmorphes.ReSetUVList(host.Connector, form);
                this.Render();

            };

            form.SendModelButton.Click += (o, args) =>
            {
                this.mypmx.SendModel(host.Connector);
            };

            form.MoveButton.Click += (o, args) => { DrivingMode.Move.GetInstance().ButtonClicked(this, this.form.MoveValue); };
            form.ScaleButton.Click += (o, args) => { DrivingMode.Scale.GetInstance().ButtonClicked(this, this.form.ScaleValue, this.form.CenterValue); };
            form.RotButton.Click += (o, args) => { DrivingMode.Rotate.GetInstance().ButtonClicked(this, this.form.RotValue, this.form.CenterValue); };

            form.ReadMorph.Click += (o, args) =>
            {
                this.uvmorphes.ReadMorph(host.Connector, this.mypmx, this.vS, form);
                this.form.MorphSlider.Value = this.form.MorphSlider.Maximum;
                ResetMeshVertex();
                ResetSelectedBuffer();
                Render();
            };
            form.SetMorph.Click += (o, args) =>
            {
                this.uvmorphes.MakeMorph(host.Connector, this.mypmx, this.vS, form);
                this.form.MorphSlider.Value = this.form.MorphSlider.Maximum;
                ResetMeshVertex();
                ResetSelectedBuffer();
                Render();
            };
            form.AddMorph.Click += (o, args) => { this.uvmorphes.AddMorph(host, form); };
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
            pps = new PresentParameters[]
            {
                new PresentParameters
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
                },
                new PresentParameters
                {
                    BackBufferFormat = Format.X8R8G8B8,
                    BackBufferCount = 1,
                    BackBufferWidth = subform.ClientSize.Width,
                    BackBufferHeight = subform.ClientSize.Height,
                    Multisample = MultisampleType.None,
                    SwapEffect = SwapEffect.Discard,
                    EnableAutoDepthStencil = true,
                    AutoDepthStencilFormat = Format.D16,
                    PresentFlags = PresentFlags.DiscardDepthStencil,
                    PresentationInterval = PresentInterval.Default,
                    Windowed = true,
                    DeviceWindowHandle = subform.Handle
                }
            };
            //Direct3Dオブジェクト・デバイスを作成
            direct3D = new Direct3D();
            try
            {
                device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, pps);
                this.form.SetDriveState("松");
            }
            catch (Direct3D9Exception ex1)
            {
                try
                {
                    device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.SoftwareVertexProcessing, pps);
                    this.form.SetDriveState("竹\r\n" + ex1.Source + "\r\n" + ex1.Message);
                }
                catch (Direct3D9Exception ex2)
                {
                    try
                    {
                        device = new Device(direct3D, 0, DeviceType.Reference, form.Handle, CreateFlags.SoftwareVertexProcessing, pps);
                        this.form.SetDriveState("梅\r\n" + ex2.Source + "\r\n" + ex2.Message);
                    }
                    catch (Direct3D9Exception ex3)
                    {
                        MessageBox.Show(ex3.Message + "\r\nデバイスの作成に失敗しました。", "DirectX Initialization failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        form.Close();
                    }
                }
            }
            //mainSwap = device.GetSwapChain(0);
            //subSwap = new SwapChain(device, pps[1]);
            this.camera = new Camera(form.ViewPanel,-1);
            //高さと幅設定

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
            subform.Show();
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
            device.SetTransform(TransformState.Projection, this.camera.TransformProjection(device,1));
            //ビュー
            device.SetTransform(TransformState.View, camera.TransformView());

            //マテリアル設定
            //device.Material = new Material() { Diffuse = new Color4(Color.GhostWhite) };

            //カリング無視
            this.device.SetRenderState(RenderState.CullMode, Cull.None);

            //アルファブレンド
            this.device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
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
                Console.WriteLine("デバイスロスト");
                if (device.TestCooperativeLevel() == ResultCode.DeviceNotReset)
                {
                    device.Reset(pps);
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
            //    if (mainSwap.Disposed)
            //    {
            //        mainSwap = new SwapChain(device, pps[0]);
            //    }
            //    using (Surface surface = mainSwap.GetBackBuffer(0))
            //    {
            //        // デバイスのレンダリングターゲットの設定
            //        this.device.SetRenderTarget(0, surface);
            //    }

                if (KeyBoardEvent.GetInstance().OnKeys[(int)Keys.Enter]) { this.camera.ResetCamera(); MessageBox.Show("called"); }
                device.SetTransform(TransformState.View, this.camera.TransformView());
                device.SetTransform(TransformState.Projection, this.camera.TransformProjection(device,this.textureinfoes[vS.SelectedMaterial].Aspect));

                //描画を開始
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, mmanage.BGColor, 1.0f, 0);
                device.BeginScene();

                //Xファイル
                device.Material = mmanage.ForXFile;
                device.SetRenderState(RenderState.Lighting, true);
                device.SetRenderState(RenderState.FillMode,FillMode.Wireframe);
                mesh.DrawSubset(vS.SelectedMaterial);

                //背景表示
                device.Material = mmanage.ForTexBackGround;
                device.SetTexture(0, textures[vS.SelectedMaterial]);
                device.SetRenderState(RenderState.FillMode, FillMode.Solid);
                device.SetStreamSource(0, texvertex, 0, CustomVertex.PositionTextured.Stride);
                device.VertexFormat = CustomVertex.PositionTextured.FVF;
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);


                //頂点表示
                device.Material = mmanage.ForVertexPoint;
                device.SetRenderState(RenderState.FillMode, FillMode.Point);
                device.SetRenderState(RenderState.PointSpriteEnable, true);
                device.SetRenderState(RenderState.PointSize, 6f);
                device.SetTexture(0, null);
                mesh.DrawSubset(this.vS.SelectedMaterial);

                //選択頂点表示
                if (vS.selectedVertexIndex.Length > 0)
                {
                    device.Material = mmanage.ForSelectedPoint;
                    device.SetRenderState(RenderState.FillMode, FillMode.Solid);
                    device.SetRenderState(RenderState.PointSpriteEnable, true);
                    device.SetStreamSource(0, selectedbuffer, 0, CustomVertex.PositionOnly.Stride);
                    device.VertexFormat = CustomVertex.PositionOnly.FVF;
                    device.DrawPrimitives(PrimitiveType.PointList,
                        0,
                        vS.selectedVertexIndex.Length);
                }

                //選択領域表示
                if ((this.drivingMode.Type() == DrawingMode.ADD)
                    || (this.drivingMode.Type() == DrawingMode.REMOVE)
                    || (this.drivingMode.Type() == DrawingMode.SELECT))
                {
                    device.Material = mmanage.ForSelectedArea;
                    device.SetRenderState(RenderState.FillMode, FillMode.Solid);
                    device.VertexFormat = CustomVertex.PositionColored.FVF;
                    device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, this.vS.SelectPoly);
                }

                if (this.vS.IsNearUsed)
                {
                    CustomVertex.PositionOnly[] nearva = { mypmx.VertexArray[this.vS.NearUsedIndex] };
                    device.Material = mmanage.ForNearVertex;
                    device.DrawUserPrimitives(PrimitiveType.PointList, 1, nearva);
                }
                device.EndScene();
                device.Present();
                //mainSwap.Present(Present.None);
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
            if (form != null) form.Dispose();
            if (message != null) message.Dispose();
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