using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PEPlugin;
using PEPlugin.Pmx;
using PEPlugin.SDX;
using SlimDX;

namespace MyUVEditor
{
    class MakeUVMorph
    {
        Dictionary<int,Vector3> basePos = new Dictionary<int,Vector3>();
        Dictionary<int, Vector3> morphedPos = new Dictionary<int, Vector3>();
        public int[] GetMorphTargetIndex() { return this.basePos.Keys.ToArray();}
        
        public MakeUVMorph(IPEConnector connector, Form1 form)
        {
            ReSetUVList(connector, form);
        }

        public void ReSetUVList(IPEConnector connector,Form1 form)
        {
            var pmx = connector.Pmx.GetCurrentState();
            List<string> names = new List<string>();
            for (int i = 0; i < pmx.Morph.Count;i++ )
                {
                    if (pmx.Morph[i].IsUV)
                    {
                        names.Add(i.ToString()+":"+pmx.Morph[i].Name);
                    }
                }
            form.ListUpUVMorph(names.ToArray());
        }

        public void MakeMorph(IPEConnector connector,MyPMX mypmx,VertexSelect vs,Form1 form)
        {
            float diff = form.Morphdiffernce;
            IPXPmx pmx = connector.Pmx.GetCurrentState();
            if (mypmx.VertexArray.Count() != pmx.Vertex.Count)
            {
                MessageBox.Show("本体とプラグインのモデル頂点数が異なるのでこの操作はキャンセルされました。");
                return;
            }

            basePos.Clear();
            morphedPos.Clear();
            for (int i = 0; i < pmx.Vertex.Count;i++ )
            {
                Vector3 morphPosI = mypmx.VertexArray[i].Position;
                Vector3 basePosI = new Vector3( pmx.Vertex[i].UV.U,pmx.Vertex[i].UV.V,0);
                if (System.Math.Abs(basePosI.X - morphPosI.X) > diff || System.Math.Abs(basePosI.Y - morphPosI.Y) > diff) { basePos.Add(i, basePosI); morphedPos.Add(i, morphPosI); }
            }
            form.SetMorphedCount(basePos.Count);
            vs.selectedVertexIndex = this.GetMorphTargetIndex();
        }

        public void ReadMorph(IPEConnector connector, MyPMX mypmx, VertexSelect vs,Form1 form)
        {
            IPXPmx pmx = connector.Pmx.GetCurrentState();
            if (mypmx.VertexArray.Count() != pmx.Vertex.Count)
            {
                MessageBox.Show("本体とプラグインのモデル頂点数が異なるのでこの操作はキャンセルされました。");
                return;
            }

            basePos.Clear();
            morphedPos.Clear();
            int morphindex = form.SelectedUVMorph();
            if (morphindex < 0) { MessageBox.Show("UVモーフが選べません"); return; }
            var uvMorph = pmx.Morph[morphindex];
            foreach (IPXUVMorphOffset offset in uvMorph.Offsets)
            {
                IPXVertex v = offset.Vertex;
                int index = pmx.Vertex.IndexOf(v);
                Vector3 baseI = new Vector3(v.UV.U, v.UV.V, 0);
                Vector3 morphI = baseI + new Vector3(offset.Offset.X,offset.Offset.Y,0);
                try
                {
                    this.basePos.Add(index, baseI);
                    this.morphedPos.Add(index, morphI);
                }
                catch { MessageBox.Show(index.ToString()); throw; }
            }
            form.SetMorphedCount(basePos.Count);
            vs.selectedVertexIndex = this.GetMorphTargetIndex();
        }



        public void MoveMorph(MyPMX mypmx, float ratio)
        {

            foreach (var i in basePos.Keys)
            {
                mypmx.VertexArray[i].Position = (1 - ratio) * basePos[i] + ratio * morphedPos[i];
            }
        }

        public void AddMorph(IPEPluginHost host,Form1 form)
        {
            if (basePos.Count < 1) { MessageBox.Show("モデル形状に変化がありません"); return; }
            IPXMorph nmorph = host.Builder.Pmx.Morph();
            nmorph.Kind = MorphKind.UV;
            nmorph.Name = form.MorphName;
            nmorph.Panel = form.MorphPanel;
            var pmx = host.Connector.Pmx.GetCurrentState();
            foreach (var i in basePos.Keys)
            {
                IPXUVMorphOffset offset = host.Builder.Pmx.UVMorphOffset();
                offset.Vertex = pmx.Vertex[i];
                Vector3 offsetVec = morphedPos[i]-basePos[i];
                offset.Offset = new V4(offsetVec, 0);
                nmorph.Offsets.Add(offset);
            }
            pmx.Morph.Add(nmorph);
            host.Connector.Pmx.Update(pmx);
            host.Connector.Form.UpdateList(PEPlugin.Pmd.UpdateObject.All);
            host.Connector.View.PMDView.UpdateModel();
            host.Connector.View.PMDView.UpdateView();
            ReSetUVList(host.Connector,form);
            MessageBox.Show(nmorph.Name + "を追加しました");
        }
    }
}
