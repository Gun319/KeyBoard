using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace KeyBoard.Util
{
    /// <summary>
    /// 监控
    /// </summary>
    public static class KeyboardInput
    {
        /// <summary>
        /// 按下带有修改键的键
        /// </summary>
        public static void SendKey(IEnumerable<int> modifierKeys, int key)
        {
            if (key <= 0)
            {
                return;
            }

            if (modifierKeys == null || modifierKeys.Count() == 0)
            {
                NativeMethods.INPUT[] inputs = new NativeMethods.INPUT[1];
                inputs[0].type = NativeMethods.INPUT_KEYBOARD;
                inputs[0].inputUnion.ki.wVk = (short)key;
                SendInput(1, inputs, Marshal.SizeOf(inputs[0]));
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint SendInput(uint nInputs, NativeMethods.INPUT[] pInputs, int cbSize);
    }
}
