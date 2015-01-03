using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyUVEditor
{
    class KeyBoardEvent
    {
        private static KeyBoardEvent instance= new KeyBoardEvent();
        private KeyBoardEvent() { }
        public static KeyBoardEvent GetInstance() { return instance; }
        public bool IsShift { get { return this.OnKeys[(int)Keys.ShiftKey]; } }
        public bool IsCtrl { get { return this.OnKeys[(int)Keys.ControlKey]; } }

        public bool[] OnKeys = new bool[256];

        public void CreateInputEvent(Control control)
        {
            control.KeyDown += new KeyEventHandler(form_KeyDown);
            control.KeyUp += new KeyEventHandler(form_KeyUp);
        }

        /// <summary>
        /// キーボードのキーを押した瞬間押されたキーのフラグを建てる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode < OnKeys.Length)
            {
                OnKeys[(int)e.KeyCode] = true;
            }

            var vp = (Control)sender;
            var fm = (Form1)vp.Parent.Parent;
            fm.ChangeStateKey(e.KeyValue);

        }

        /// <summary>
        /// キーボードのキーを離した瞬間キーのフラグを降ろす
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void form_KeyUp(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode < OnKeys.Length)
            {
                OnKeys[(int)e.KeyCode] = false;
            }
        }

    }
}
