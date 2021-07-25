using System.Windows.Forms;
using Button = System.Windows.Controls.Button;

namespace KeyBoard
{
    public class KeyButton : Button
    {
        /// <summary>
        /// 键码
        /// </summary>
        public int KeyCode { get; set; }

        Keys key;
        public Keys Key
        {
            get
            {
                if (key == Keys.None)
                {
                    key = (Keys)KeyCode;
                }
                return key;
            }
        }

        /// <summary>
        /// 指定它是否为字母键
        /// </summary>
        public bool IsLetterKey
        {
            get
            {
                return Key >= Keys.A && Key <= Keys.Z;
            }
        }
    }
}
