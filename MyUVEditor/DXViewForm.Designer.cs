namespace MyUVEditor
{
    partial class DXViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dxView1 = new MyUVEditor.DXView();
            this.SuspendLayout();
            // 
            // dxView1
            // 
            this.dxView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dxView1.Location = new System.Drawing.Point(0, 0);
            this.dxView1.Name = "dxView1";
            this.dxView1.Size = new System.Drawing.Size(292, 273);
            this.dxView1.TabIndex = 0;
            // 
            // DXViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.dxView1);
            this.Name = "DXViewForm";
            this.Text = "DXViewForm";
            this.ResumeLayout(false);

        }

        #endregion

        protected DXView dxView1;

    }
}