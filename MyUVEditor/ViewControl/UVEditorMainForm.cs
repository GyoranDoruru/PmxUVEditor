using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MyUVEditor.Camera;
using SlimDX;
using PEPlugin.Pmx;

namespace MyUVEditor
{
    public partial class UVEditorMainForm : Form
    {
        public Panel ViewPanel { get { return splitContainer1.Panel1; } }
        public ComboBox SelectedMaterial { get { return _materialSelectbox; } }
        public Button ReceiveSelectedButton { get { return this._selectrecieve; } }
        public Button SendSelectedButton { get { return this._selectsend; } }
        public Button ReceiveModelButton { get { return this._recievemodel; } }
        public Button SendModelButton { get { return this._sendmodel; } }

        private Vector2 TextBox2Number(TextBox tb1,TextBox tb2)
        {
            try { return new Vector2(float.Parse(tb1.Text), float.Parse(tb2.Text)); }
            catch{ return new Vector2(); }
        }
        private float TextBox2Number(TextBox tb1)
        {
            try { return float.Parse(tb1.Text); }
            catch { return 0; }
        }

        public Button MoveButton { get { return this.button_move; } }
        public Vector2 MoveValue { get { return this.TextBox2Number(this.ValueTextX, this.ValueTextY); } }
        public Button ScaleButton { get { return this.button_scale; } }
        public Vector2 ScaleValue { get { return this.TextBox2Number(this.ScaleX, this.ScaleY); } }
        public Button RotButton { get { return this.button_rot; } }
        public float RotValue { get { return (float)(this.TextBox2Number(this.Angle)*System.Math.PI/180); } }
        public Vector2 CenterValue { get { return this.TextBox2Number(this.CenterX, this.CenterY); } }
        public ToolStripMenuItem UnDo { get { return _undoToolStripMenuItem; } }
        public ToolStripMenuItem ReDo { get { return redoToolStripMenuItem; } }
        public ToolStripMenuItem SelectAll { get { return this.selectAll_ToolStripMenuItem; } }
        public Vector3 UVLocation { set { this.toolStripStatusLabel2.Text = "( " + value.X + ", " + value.Y + " )"; } }
        public Stack<UnDoRedoRecord> undoStack = new Stack<UnDoRedoRecord>();
        public Stack<UnDoRedoRecord> redoStack = new Stack<UnDoRedoRecord>();
        public TabControl TabControll { get { return this.tabControl1; } }

        public bool IsRotateState { get { return this.radioButton3.Checked; } }
        public bool IsScaleState { get { return this.radioButton2.Checked; } }
        public void ChangeStateKey(int key)
        {
            switch (key)
            {
                case (int)Keys.S: this.radioButton1.Checked = true; break;
                case (int)Keys.Q: this.radioButton2.Checked = true; break;
                case (int)Keys.R: this.radioButton3.Checked = true; break;
            }
        }

        private string version;

        //UVモーフ用コントロールプロパティ
        public int SelectedUVMorph()
        {
            try
            {
                string[] texts = ((string)(this.morph_comboBox.SelectedItem)).Split(':');
                return int.Parse(texts[0]);
            }
            catch (FormatException fe) { MessageBox.Show(fe.Message); return -1; }
            catch { throw; }
        }
        public void SetMorphedCount(int i) { this.label6.Text = "相違頂点数 : " + i.ToString(); }
        public float Morphdiffernce { get { return this.TextBox2Number(this.morphDif_textBox);} }
        public string MorphName { get { return this.newMorphName_textBox.Text; } }
        public int MorphPanel { get { return this.morphPanel_comboBox.SelectedIndex; } }
        public Button ReadMorph { get { return this.getMorph_button; } }
        public Button SetMorph { get { return this.setMorph_button; } }
        public Button AddMorph { get { return this.addNewMorph_button; } }
        public TrackBar MorphSlider { get { return this.trackBar1; } }

        public ICamera Camera { get; set; }

        public UVEditorMainForm(string version)
        {
            InitializeComponent();
            this.version = version;
            this.Text += version;
            this.MouseWheel += new MouseEventHandler(viewpanel_MouseWheel);
            this.morphPanel_comboBox.SelectedIndex = 1;
        }

        internal DirectX11.ICommonContents CommonContents { get; set; }

        public void SetList()
        {
            var pmx = CommonContents.Pmx;
            this._materialSelectbox.Items.Clear();
            foreach(var m in pmx.Material)
            {
                string tag = this._materialSelectbox.Items.Count + ": " + m.Name;
                this._materialSelectbox.Items.Add(tag);

            }
            this._materialSelectbox.SelectedIndex = 0;

            this.morph_comboBox.Items.Clear();
            foreach(var m in pmx.Morph)
            {
                if (!m.IsUV)
                    continue;
                string tag = this.morph_comboBox.Items.Count + ": " + m.Name;
                this.morph_comboBox.Items.Add(m.Name);
            }

            if (morph_comboBox.Items.Count < 1)
                this.morph_comboBox.Enabled = false;
            else
                this.morph_comboBox.SelectedIndex = 0;
        }

        public void ListUpUVMorph(string[] names)
        {
            this.morph_comboBox.Items.Clear();
            foreach (var name in names) this.morph_comboBox.Items.Add(name);
        }

        public void Undo(MyPMX mypmx)
        {
            if (undoStack.Count > 0)
            {
                UnDoRedoRecord poppedout = undoStack.Pop();
                redoStack.Push(new UnDoRedoRecord(poppedout._index, mypmx));
                if (undoStack.Count <= 0) this._undoToolStripMenuItem.Enabled = false;
                this.redoToolStripMenuItem.Enabled = true;
                this.EditMyPMXVertexes(mypmx, poppedout);
            }
        }

        public void Redo(MyPMX mypmx)
        {
            if (redoStack.Count > 0)
            {
                UnDoRedoRecord poppedout = redoStack.Pop();
                undoStack.Push(new UnDoRedoRecord(poppedout._index, mypmx));
                if (redoStack.Count <= 0) this.redoToolStripMenuItem.Enabled = false;
                this._undoToolStripMenuItem.Enabled = true;
                this.EditMyPMXVertexes(mypmx, poppedout);
            }

        }

        private void EditMyPMXVertexes(MyPMX mypmx, UnDoRedoRecord record)
        {
            for (int i = 0; i < record._index.Length; i++)
            {
                mypmx.VertexArray[record._index[i]].Position = record._vec[i];
            }
        }

        /// <summary>
        /// アンドゥをスタックする
        /// </summary>
        /// <param name="pushed"></param>
        public void PushUndo(UnDoRedoRecord pushed) 
        {
            undoStack.Push(pushed);
            redoStack = new Stack<UnDoRedoRecord>();
            this._undoToolStripMenuItem.Enabled = true;
            this.redoToolStripMenuItem.Enabled = false;
        }

        public void ResetUndo()
        {
            this.undoStack.Clear();
            this.redoStack.Clear();
            this._undoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Enabled = false;
        }

        #region イベント
        private void viewpanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Camera == null || Camera.IsDisposed)
                return;
            UVLocation = Camera.ScreenToWorldVec(e.Location);
        }

        private void viewpanel_Click(object sender, EventArgs e)
        {
            if (!splitContainer1.Panel1.Focused) { splitContainer1.Panel1.Focus(); }
        }
        private void splitContainer1_Panel1_Resize(object sender, EventArgs e) { }
        public void _materialSelectbox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void _selectrecieve_Click(object sender, EventArgs e) { }
        private void _selectsend_Click(object sender, EventArgs e) { }
        private void _recievemodel_Click(object sender, EventArgs e) { }
        private void _sendmodel_Click(object sender, EventArgs e) { }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void selectAll_ToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void infoIToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show(GetInfo()); }
        private void button_move_Click(object sender, EventArgs e) { }
        private void button_scale_Click(object sender, EventArgs e) { }
        private void button_rot_Click(object sender, EventArgs e) { }
        private void getMorph_button_Click(object sender, EventArgs e) { }
        private void setMorph_button_Click(object sender, EventArgs e) { }
        private void trackBar1_Scroll(object sender, EventArgs e) { }
        #endregion

        private string GetInfo()
        {
            string info = "Version: " + version + "\r\n";
            info += "Sampling Count: " + DirectX11.DeviceManager11.SampleCount + "\r\n";
            info += "Sampling Quality: " + DirectX11.DeviceManager11.SampleQuality + "\r\n";
            return info;
        }


    }
}
