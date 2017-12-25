namespace MyUVEditor
{
    partial class UVEditorMainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._recievemodel = new System.Windows.Forms.Button();
            this._sendmodel = new System.Windows.Forms.Button();
            this._selectrecieve = new System.Windows.Forms.Button();
            this._selectsend = new System.Windows.Forms.Button();
            this._materialSelectbox = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_rot = new System.Windows.Forms.Button();
            this.Angle = new System.Windows.Forms.TextBox();
            this.button_scale = new System.Windows.Forms.Button();
            this.ScaleX = new System.Windows.Forms.TextBox();
            this.ScaleY = new System.Windows.Forms.TextBox();
            this.button_move = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.CenterX = new System.Windows.Forms.TextBox();
            this.CenterY = new System.Windows.Forms.TextBox();
            this.ValueTextX = new System.Windows.Forms.TextBox();
            this.ValueTextY = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.getMorph_button = new System.Windows.Forms.Button();
            this.morph_comboBox = new System.Windows.Forms.ComboBox();
            this.addNewMorph_button = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.morphDif_textBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.setMorph_button = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.morphPanel_comboBox = new System.Windows.Forms.ComboBox();
            this.newMorphName_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ファイルToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.編集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAll_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ヘルプHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _recievemodel
            // 
            this._recievemodel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recievemodel.Location = new System.Drawing.Point(6, 331);
            this._recievemodel.Name = "_recievemodel";
            this._recievemodel.Size = new System.Drawing.Size(154, 45);
            this._recievemodel.TabIndex = 1;
            this._recievemodel.Text = "本体からモデル受信";
            this._recievemodel.UseVisualStyleBackColor = true;
            this._recievemodel.Click += new System.EventHandler(this._recievemodel_Click);
            // 
            // _sendmodel
            // 
            this._sendmodel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._sendmodel.Location = new System.Drawing.Point(6, 383);
            this._sendmodel.Name = "_sendmodel";
            this._sendmodel.Size = new System.Drawing.Size(154, 45);
            this._sendmodel.TabIndex = 2;
            this._sendmodel.Text = "本体へモデル送信";
            this._sendmodel.UseVisualStyleBackColor = true;
            this._sendmodel.Click += new System.EventHandler(this._sendmodel_Click);
            // 
            // _selectrecieve
            // 
            this._selectrecieve.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._selectrecieve.Location = new System.Drawing.Point(6, 229);
            this._selectrecieve.Name = "_selectrecieve";
            this._selectrecieve.Size = new System.Drawing.Size(154, 45);
            this._selectrecieve.TabIndex = 3;
            this._selectrecieve.Text = "選択頂点受信";
            this._selectrecieve.UseVisualStyleBackColor = true;
            this._selectrecieve.Click += new System.EventHandler(this._selectrecieve_Click);
            // 
            // _selectsend
            // 
            this._selectsend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._selectsend.Location = new System.Drawing.Point(6, 280);
            this._selectsend.Name = "_selectsend";
            this._selectsend.Size = new System.Drawing.Size(155, 45);
            this._selectsend.TabIndex = 4;
            this._selectsend.Text = "選択頂点送信";
            this._selectsend.UseVisualStyleBackColor = true;
            this._selectsend.Click += new System.EventHandler(this._selectsend_Click);
            // 
            // _materialSelectbox
            // 
            this._materialSelectbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._materialSelectbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._materialSelectbox.FormattingEnabled = true;
            this._materialSelectbox.Location = new System.Drawing.Point(5, 25);
            this._materialSelectbox.Name = "_materialSelectbox";
            this._materialSelectbox.Size = new System.Drawing.Size(171, 20);
            this._materialSelectbox.TabIndex = 5;
            this._materialSelectbox.SelectedIndexChanged += new System.EventHandler(this._materialSelectbox_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Click += new System.EventHandler(this.viewpanel_Click);
            this.splitContainer1.Panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.viewpanel_MouseWheel);
            this.splitContainer1.Panel1MinSize = 1;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this._materialSelectbox);
            this.splitContainer1.Panel2MinSize = 145;
            this.splitContainer1.Size = new System.Drawing.Size(871, 520);
            this.splitContainer1.SplitterDistance = 688;
            this.splitContainer1.TabIndex = 6;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 51);
            this.tabControl1.MinimumSize = new System.Drawing.Size(175, 462);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(175, 466);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this._sendmodel);
            this.tabPage1.Controls.Add(this._selectsend);
            this.tabPage1.Controls.Add(this._selectrecieve);
            this.tabPage1.Controls.Add(this._recievemodel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(167, 440);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "編集";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(6, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(154, 83);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "頂点操作";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(7, 62);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(67, 16);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "回転 (R)";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(7, 40);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(97, 16);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "拡大・縮小 (Q)";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(7, 18);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(96, 16);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "選択/移動 (S)";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.button_rot);
            this.groupBox2.Controls.Add(this.Angle);
            this.groupBox2.Controls.Add(this.button_scale);
            this.groupBox2.Controls.Add(this.ScaleX);
            this.groupBox2.Controls.Add(this.ScaleY);
            this.groupBox2.Controls.Add(this.button_move);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.CenterX);
            this.groupBox2.Controls.Add(this.CenterY);
            this.groupBox2.Controls.Add(this.ValueTextX);
            this.groupBox2.Controls.Add(this.ValueTextY);
            this.groupBox2.Location = new System.Drawing.Point(6, 92);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(154, 122);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "値入力で操作";
            // 
            // button_rot
            // 
            this.button_rot.Location = new System.Drawing.Point(7, 68);
            this.button_rot.Name = "button_rot";
            this.button_rot.Size = new System.Drawing.Size(43, 19);
            this.button_rot.TabIndex = 11;
            this.button_rot.Text = "回転";
            this.button_rot.UseVisualStyleBackColor = true;
            this.button_rot.Click += new System.EventHandler(this.button_rot_Click);
            // 
            // Angle
            // 
            this.Angle.Location = new System.Drawing.Point(53, 68);
            this.Angle.Name = "Angle";
            this.Angle.Size = new System.Drawing.Size(39, 19);
            this.Angle.TabIndex = 10;
            this.Angle.Text = "0";
            this.Angle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // button_scale
            // 
            this.button_scale.Location = new System.Drawing.Point(7, 43);
            this.button_scale.Name = "button_scale";
            this.button_scale.Size = new System.Drawing.Size(43, 19);
            this.button_scale.TabIndex = 9;
            this.button_scale.Text = "拡大";
            this.button_scale.UseVisualStyleBackColor = true;
            this.button_scale.Click += new System.EventHandler(this.button_scale_Click);
            // 
            // ScaleX
            // 
            this.ScaleX.Location = new System.Drawing.Point(53, 43);
            this.ScaleX.Name = "ScaleX";
            this.ScaleX.Size = new System.Drawing.Size(39, 19);
            this.ScaleX.TabIndex = 8;
            this.ScaleX.Text = "1";
            this.ScaleX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ScaleY
            // 
            this.ScaleY.Location = new System.Drawing.Point(98, 43);
            this.ScaleY.Name = "ScaleY";
            this.ScaleY.Size = new System.Drawing.Size(39, 19);
            this.ScaleY.TabIndex = 7;
            this.ScaleY.Text = "1";
            this.ScaleY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // button_move
            // 
            this.button_move.Location = new System.Drawing.Point(7, 18);
            this.button_move.Name = "button_move";
            this.button_move.Size = new System.Drawing.Size(43, 19);
            this.button_move.TabIndex = 6;
            this.button_move.Text = "移動";
            this.button_move.UseVisualStyleBackColor = true;
            this.button_move.Click += new System.EventHandler(this.button_move_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "中心";
            // 
            // CenterX
            // 
            this.CenterX.Location = new System.Drawing.Point(53, 93);
            this.CenterX.Name = "CenterX";
            this.CenterX.Size = new System.Drawing.Size(39, 19);
            this.CenterX.TabIndex = 4;
            this.CenterX.Text = "0";
            this.CenterX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // CenterY
            // 
            this.CenterY.Location = new System.Drawing.Point(98, 93);
            this.CenterY.Name = "CenterY";
            this.CenterY.Size = new System.Drawing.Size(39, 19);
            this.CenterY.TabIndex = 3;
            this.CenterY.Text = "0";
            this.CenterY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ValueTextX
            // 
            this.ValueTextX.Location = new System.Drawing.Point(53, 18);
            this.ValueTextX.Name = "ValueTextX";
            this.ValueTextX.Size = new System.Drawing.Size(39, 19);
            this.ValueTextX.TabIndex = 1;
            this.ValueTextX.Text = "0";
            this.ValueTextX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ValueTextY
            // 
            this.ValueTextY.Location = new System.Drawing.Point(98, 18);
            this.ValueTextY.Name = "ValueTextY";
            this.ValueTextY.Size = new System.Drawing.Size(39, 19);
            this.ValueTextY.TabIndex = 0;
            this.ValueTextY.Text = "0";
            this.ValueTextY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.addNewMorph_button);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(167, 440);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "UVモーフ";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.getMorph_button);
            this.groupBox5.Controls.Add(this.morph_comboBox);
            this.groupBox5.Location = new System.Drawing.Point(6, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(154, 83);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "既存モーフから取得";
            // 
            // getMorph_button
            // 
            this.getMorph_button.Location = new System.Drawing.Point(72, 44);
            this.getMorph_button.Name = "getMorph_button";
            this.getMorph_button.Size = new System.Drawing.Size(75, 23);
            this.getMorph_button.TabIndex = 1;
            this.getMorph_button.Text = "取得";
            this.toolTip1.SetToolTip(this.getMorph_button, "本体モデルから指定したUVモーフを取得します");
            this.getMorph_button.UseVisualStyleBackColor = true;
            this.getMorph_button.Click += new System.EventHandler(this.getMorph_button_Click);
            // 
            // morph_comboBox
            // 
            this.morph_comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.morph_comboBox.FormattingEnabled = true;
            this.morph_comboBox.Location = new System.Drawing.Point(6, 18);
            this.morph_comboBox.Name = "morph_comboBox";
            this.morph_comboBox.Size = new System.Drawing.Size(141, 20);
            this.morph_comboBox.TabIndex = 0;
            // 
            // addNewMorph_button
            // 
            this.addNewMorph_button.Location = new System.Drawing.Point(7, 387);
            this.addNewMorph_button.Name = "addNewMorph_button";
            this.addNewMorph_button.Size = new System.Drawing.Size(153, 47);
            this.addNewMorph_button.TabIndex = 2;
            this.addNewMorph_button.Text = "新規追加";
            this.addNewMorph_button.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.trackBar1);
            this.groupBox4.Controls.Add(this.morphDif_textBox);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.setMorph_button);
            this.groupBox4.Location = new System.Drawing.Point(7, 218);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(155, 146);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "動作確認";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "相違頂点数 : 0";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(6, 57);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(143, 45);
            this.trackBar1.TabIndex = 3;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Value = 100;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // morphDif_textBox
            // 
            this.morphDif_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.morphDif_textBox.Location = new System.Drawing.Point(89, 31);
            this.morphDif_textBox.Name = "morphDif_textBox";
            this.morphDif_textBox.Size = new System.Drawing.Size(60, 19);
            this.morphDif_textBox.TabIndex = 2;
            this.morphDif_textBox.Text = "0.00001\r\n";
            this.morphDif_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(88, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "閾値";
            // 
            // setMorph_button
            // 
            this.setMorph_button.Location = new System.Drawing.Point(6, 27);
            this.setMorph_button.Name = "setMorph_button";
            this.setMorph_button.Size = new System.Drawing.Size(75, 23);
            this.setMorph_button.TabIndex = 0;
            this.setMorph_button.Text = "反映";
            this.toolTip1.SetToolTip(this.setMorph_button, "本体モデルとプラグインモデルの差分からUVモーフの動作を設定します");
            this.setMorph_button.UseVisualStyleBackColor = true;
            this.setMorph_button.Click += new System.EventHandler(this.setMorph_button_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.morphPanel_comboBox);
            this.groupBox3.Controls.Add(this.newMorphName_textBox);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(6, 95);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(155, 117);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "パラメータ";
            // 
            // morphPanel_comboBox
            // 
            this.morphPanel_comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.morphPanel_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.morphPanel_comboBox.FormattingEnabled = true;
            this.morphPanel_comboBox.Items.AddRange(new object[] {
            "非表示",
            "左下（まゆ）",
            "左上（目）",
            "右上（ﾘｯﾌﾟ）",
            "右下（その他）"});
            this.morphPanel_comboBox.Location = new System.Drawing.Point(57, 43);
            this.morphPanel_comboBox.Name = "morphPanel_comboBox";
            this.morphPanel_comboBox.Size = new System.Drawing.Size(90, 20);
            this.morphPanel_comboBox.TabIndex = 3;
            // 
            // newMorphName_textBox
            // 
            this.newMorphName_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.newMorphName_textBox.Location = new System.Drawing.Point(57, 18);
            this.newMorphName_textBox.Name = "newMorphName_textBox";
            this.newMorphName_textBox.Size = new System.Drawing.Size(90, 19);
            this.newMorphName_textBox.TabIndex = 2;
            this.newMorphName_textBox.Text = "新規UVモーフ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "パネル : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "名称 : ";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "材質";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルToolStripMenuItem,
            this.編集ToolStripMenuItem,
            this.ヘルプHToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(871, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ファイルToolStripMenuItem
            // 
            this.ファイルToolStripMenuItem.Enabled = false;
            this.ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
            this.ファイルToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.ファイルToolStripMenuItem.Text = "ファイル(&F)";
            // 
            // 編集ToolStripMenuItem
            // 
            this.編集ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.selectAll_ToolStripMenuItem});
            this.編集ToolStripMenuItem.Name = "編集ToolStripMenuItem";
            this.編集ToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.編集ToolStripMenuItem.Text = "編集(&E)";
            // 
            // _undoToolStripMenuItem
            // 
            this._undoToolStripMenuItem.Enabled = false;
            this._undoToolStripMenuItem.Name = "_undoToolStripMenuItem";
            this._undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this._undoToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this._undoToolStripMenuItem.Text = "元に戻す(&U)";
            this._undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.redoToolStripMenuItem.Text = "やりなおす(&R)";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // selectAll_ToolStripMenuItem
            // 
            this.selectAll_ToolStripMenuItem.Name = "selectAll_ToolStripMenuItem";
            this.selectAll_ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAll_ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.selectAll_ToolStripMenuItem.Text = "すべて選択(&A)";
            this.selectAll_ToolStripMenuItem.Click += new System.EventHandler(this.selectAll_ToolStripMenuItem_Click);
            // 
            // ヘルプHToolStripMenuItem
            // 
            this.ヘルプHToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoIToolStripMenuItem});
            this.ヘルプHToolStripMenuItem.Name = "ヘルプHToolStripMenuItem";
            this.ヘルプHToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.ヘルプHToolStripMenuItem.Text = "ヘルプ(&H)";
            // 
            // infoIToolStripMenuItem
            // 
            this.infoIToolStripMenuItem.Name = "infoIToolStripMenuItem";
            this.infoIToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.infoIToolStripMenuItem.Text = "バージョン情報(&A)";
            this.infoIToolStripMenuItem.Click += new System.EventHandler(this.infoIToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 544);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(871, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(33, 17);
            this.toolStripStatusLabel2.Text = "(0, 0)";
            // 
            // UVEditorMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 566);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "UVEditorMainForm";
            this.Text = "UVエディタ";
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _recievemodel;
        private System.Windows.Forms.Button _sendmodel;
        private System.Windows.Forms.Button _selectrecieve;
        private System.Windows.Forms.Button _selectsend;
        private System.Windows.Forms.ComboBox _materialSelectbox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ファイルToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 編集ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ヘルプHToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoIToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_rot;
        private System.Windows.Forms.TextBox Angle;
        private System.Windows.Forms.Button button_scale;
        private System.Windows.Forms.TextBox ScaleX;
        private System.Windows.Forms.TextBox ScaleY;
        private System.Windows.Forms.Button button_move;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CenterX;
        private System.Windows.Forms.TextBox CenterY;
        private System.Windows.Forms.TextBox ValueTextX;
        private System.Windows.Forms.TextBox ValueTextY;
        private System.Windows.Forms.ToolStripMenuItem selectAll_ToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button getMorph_button;
        private System.Windows.Forms.ComboBox morph_comboBox;
        private System.Windows.Forms.Button addNewMorph_button;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TextBox morphDif_textBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button setMorph_button;
        private System.Windows.Forms.ComboBox morphPanel_comboBox;
        private System.Windows.Forms.TextBox newMorphName_textBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

