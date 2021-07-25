using System;
using System.Runtime.InteropServices;

namespace KeyBoard.Util
{
    public static class NativeMethods
    {

        // 输入结构中使用的常量
        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_HARDWARE = 2;

        // KEYBDINPUT结构中使用的常量
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;

        /// <summary>
        /// SendInput用于存储合成输入事件的信息
        /// 如 击键、鼠标移动和鼠标单击。
        /// 文档地址：http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;
            public INPUTUNION inputUnion;
        }

        /// <summary>
        /// INPUTUNION 结构只包含一个字段
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public MOUSEINPUT mi;
        }

        /// <summary>
        /// 模拟硬件事件的信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        /// <summary>
        /// 模拟键盘事件的信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;

            public int dwFlags;

            public int time;
            public IntPtr dwExtraInfo;
        }

        /// <summary>
        /// 模拟鼠标事件的信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }
    }
}