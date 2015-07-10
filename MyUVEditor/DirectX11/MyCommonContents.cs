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
            // 白テクスチャ
            m_Textures.Add("", TextureUtility.CreateWhiteTex(device));

            // 現状のカレントフォルダ
            string current = Directory.GetCurrentDirectory();

            // Toonテクスチャ
            worker.ReportProgress(0, "Toon読み込み中");
            // エディタのToonフォルダ
            string toonDir = Directory.GetParent(Args.Host.Connector.System.HostApplicationPath).FullName
                + "\\_data\\toon";
            Directory.SetCurrentDirectory(toonDir);

            worker.ReportProgress(0, "Toon読み込み中");
            string toonKey = "toon0.bmp";
            ShaderResourceView tex = TextureUtility.CreateTex(device, toonKey);
            if (tex != null)
                m_Textures.Add(toonKey, tex);
            for (int i = 1; i < 10; i++)
            {
                worker.ReportProgress(i * 10 / Pmx.Material.Count, "Toon読み込み中");
                toonKey = "toon0" + i.ToString() + ".bmp";
                tex = TextureUtility.CreateTex(device, toonKey);
                if (tex != null)
                    m_Textures.Add(toonKey, tex);
            }
            toonKey = "toon10.bmp";
            tex = TextureUtility.CreateTex(device, toonKey);
            if (tex != null)
                m_Textures.Add(toonKey, tex);

            // 材質テクスチャ
            worker.ReportProgress(0, "テクスチャ読み込み中");
            string pmxDir = Directory.GetParent(Pmx.FilePath).FullName;
            Directory.SetCurrentDirectory(pmxDir);
            int count = 0;
            foreach (var m in Pmx.Material)
            {
                string[] matTextures = { m.Tex, m.Toon, m.Sphere };
                foreach (var t in matTextures)
                {
                    if (!m_Textures.ContainsKey(t))
                    {
                        tex = TextureUtility.CreateTex(device, t);
                        if (tex != null)
                            m_Textures.Add(t, tex);
                    }
                }
                count++;
                worker.ReportProgress(count * 100 / Pmx.Material.Count, "テクスチャ読み込み中");
            }

            // カレントフォルダを元に戻す
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
