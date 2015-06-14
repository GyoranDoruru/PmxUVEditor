using System.Windows.Forms;

namespace MyUVEditor
{
    public partial class DXViewForm : Form,IDXViewForm
    {
        public DXView ViewPort { get { return this.dxView1; } }
        public DXViewForm()
        {
            InitializeComponent();
            ViewPort.SetRequest(this);
        }
        
    }
}
