﻿using System.IO;
using System.Collections.Generic;
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
        internal Dictionary<IPXVertex, int> VertexIndexDic { get; private set; }

        private IPERunArgs Args { get; set; }
        private Dictionary<string, ShaderResourceView> m_Textures;

        private void initEffect(Device device)
        {
            Effect = EffectManager11.InitEffect(device);
        }
        
        private void initTexture(Device device)
        {
            m_Textures.Add("", TextureUtility.CreateWhiteTex(device));
            string current = Directory.GetCurrentDirectory();
            string pmxDir = Directory.GetParent(Pmx.FilePath).FullName;
            Directory.SetCurrentDirectory(pmxDir);
            foreach (var m in Pmx.Material)
            {
                if (!m_Textures.ContainsKey(m.Tex))
                {
                    ShaderResourceView tex = TextureUtility.CreateTex(device, m.Tex);
                    if (tex != null)
                        m_Textures.Add(m.Tex, tex);
                }
            }
            Directory.SetCurrentDirectory(current);

        }

        public void Load(SlimDX.Direct3D11.Device device)
        {
            initEffect(device);
            initTexture(device);
            CommonDrawables = new List<IDrawable>{ getModel(device) };
        }

        private IDrawable getModel(Device device){
            var model = new DrawablePmx(Pmx);
            model.SetDrawablePmx(this);
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
