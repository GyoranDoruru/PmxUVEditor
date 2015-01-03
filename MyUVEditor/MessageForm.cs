using System.Windows.Forms;

namespace MyUVEditor
{
    public partial class MessageForm : Form
    {

        public string StatusText { get { return this.label1.Text; } set { this.label1.Text = value; } }
        public ProgressBar StatusBar { get { return this.progressBar1; } set { this.progressBar1 = value; } }
        public MessageForm()
        {
            InitializeComponent();
        }
        public void LabelReflesh(string s)
        {
            this.StatusText = s;
            this.label1.Refresh();
        }
    }
}
