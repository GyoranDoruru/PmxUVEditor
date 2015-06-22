using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using SlimDX.Direct3D11;
using PEPlugin;
using PEPlugin.Pmx;

namespace MyUVEditor.DirectX11
{
    class MyCommonContents:ICommonContents
    {
        public IPXPmx Pmx { get; private set; }
        public List<IDrawable> CommonDrawables { get; private set; }
        public Effect Effect { get; private set; }
        public BlendStateManager BlendStateManager { get; private set; }
        internal Dictionary<IPXVertex, int> VertexIndexDic { get; private set; }

        private IPERunArgs Args { get; set; }
        private Dictionary<string, ShaderResourceView> m_Textures;

        private void initEffect(Device device)
        {
            Effect = EffectManager11.InitEffect(device);
        }

        private void initBlenderState(Device device)
        {
            BlendStateManager = new BlendStateManager(device);
            device.ImmediateContext.OutputMerger.BlendState = BlendStateManager.BlendState;
        }
        
        private void initTexture(Device device, BackgroundWorker worker)
        {
            worker.ReportProgress(0, "テクスチャ読み込み中");
            m_Textures.Add("", TextureUtility.CreateWhiteTex(device));
            string current = Directory.GetCurrentDirectory();
            string pmxDir = Directory.GetParent(Pmx.FilePath).FullName;
            Directory.SetCurrentDirectory(pmxDir);
            int count = 0;
            foreach (var m in Pmx.Material)
            {
                if (!m_Textures.ContainsKey(m.Tex))
                {
                    ShaderResourceView tex = TextureUtility.CreateTex(device, m.Tex);
                    if (tex != null)
                        m_Textures.Add(m.Tex, tex);
                }
                count++;
                worker.ReportProgress(count * 100 / Pmx.Material.Count, "テクスチャ読み込み中");
            }
            Directory.SetCurrentDirectory(current);

        }

        public void Load(Device device,BackgroundWorker worker)
        {
            initEffect(device);
            initBlenderState(device);
            initTexture(device,worker);
            CommonDrawables = new List<IDrawable> { getModel(device, worker) };
        }

        private IDrawable getModel(Device device, BackgroundWorker worker){
            var model = new DrawablePmx(Pmx);
            model.SetDrawablePmx(this, worker);
            return model;
        }

        public void Unload() {
            foreach (var m in CommonDrawables)
                m.Dispose();
            CommonDrawables.Clear();
            foreach (var t in m_Textures)
                t.Value.Dispose();

            m_Textures.Clear();
            Effect.Dispose();
            BlendStateManager.Dispose();
        }

        public MyCommonContents()
        {
            m_Textures = new Dictionary<string, ShaderResourceView>();
        }

        public void SetRunArgsAndPmx(PEPlugin.IPERunArgs args)
        {
            Args = args;
            Pmx = args.Host.Connector.Pmx.GetCurrentState();
        }


        public ShaderResourceView GetTexture(string key)
        {
            ShaderResourceView value = null;
            if (m_Textures.TryGetValue(key, out value))
                return value;
            else
                return m_Textures[""];
        }
    }
}
