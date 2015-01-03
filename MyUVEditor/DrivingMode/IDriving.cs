using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyUVEditor.DrivingMode
{
    interface IDrivingMode
    {

        void MouseDowend(MyGame sender, MouseEventArgs e);
        void MouseMoved(MyGame sender, MouseEventArgs e);
        void MouseUpped(MyGame sender, MouseEventArgs e);
        DrawingMode Type();
    }
}
