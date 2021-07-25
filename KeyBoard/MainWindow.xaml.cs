using KeyBoard.Helper;
using KeyBoard.Recognize;
using KeyBoard.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using Clipboard = System.Windows.Clipboard;
using RadioButton = System.Windows.Controls.RadioButton;

namespace KeyBoard
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<int> pressedModifierKeyCodes = null;

        /// <summary>
        /// 按下的修改键(退格、清空等)
        /// </summary>
        private List<int> PressedModifierKeyCodes
        {
            get
            {
                if (pressedModifierKeyCodes == null)
                {
                    pressedModifierKeyCodes = new List<int>();
                }
                return pressedModifierKeyCodes;
            }
        }

        /// <summary>
        /// 当前窗体句柄
        /// </summary>
        private IntPtr hwnd;

        /// <summary>
        /// 数据模型
        /// </summary>
        private readonly RecognizationCore _core = new RecognizationCore();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _core;
            DrawingAttributes drawingAttributes = new DrawingAttributes();
            inkCanvas.DefaultDrawingAttributes = drawingAttributes;
            drawingAttributes.Width = 5;
            drawingAttributes.Height = 5;
            drawingAttributes.IgnorePressure = true;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            hwnd = new WindowInteropHelper(this).Handle;
            int GWL_STYLE = -16;
            int GWL_EXSTYLE = -20;
            SetWindowLong(hwnd, GWL_STYLE, (IntPtr)unchecked((int)0x94000000));
            SetWindowLong(hwnd, GWL_EXSTYLE, (IntPtr)0x08000088);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Topmost = true; // 窗口置顶
            rbCn.IsChecked = _core.IsPinyin = true;
            _core.Keys = "";
        }

        /// <summary>
        /// 移动(嵌入窗体时删除此方法)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) => DragMove();

        /// <summary>
        /// 墨迹识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WritingCanvasOnStrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            if (_core.Recognize(inkCanvas.Strokes))
            {
                borderSelect.Visibility = Visibility.Visible;
                btnMore.IsEnabled = false;
            }
        }

        /// <summary>
        /// 拼音/手写切换 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rb_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton button = sender as RadioButton;
            switch (button.Content)
            {
                case "拼音":
                    gridHandInput.Visibility = Visibility.Hidden;
                    keyBoard.Visibility = Visibility.Visible;
                    _core.IsPinyin = true;
                    Clear();
                    break;
                case "手写":
                    keyBoard.Visibility = Visibility.Hidden;
                    gridHandInput.Visibility = Visibility.Visible;
                    _core.IsPinyin = false;
                    Clear();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 判断输入方式
        /// 执行输入操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is KeyButton btn))
            {
                return;
            }

            if (_core.IsPinyin)
            {
                if (btn.IsLetterKey)
                {
                    _core.Keys += btn.Key.ToString().ToLower();
                    ShowCharatar();
                }
                else if (btn.Key == Keys.Back) // 退格
                {
                    KeyboardInput.SendKey(PressedModifierKeyCodes, btn.KeyCode);
                }
                else // 数字
                {
                    int btnKeyCode = btn.KeyCode;
                    KeyboardInput.SendKey(PressedModifierKeyCodes, btnKeyCode);
                }
            }
            else if (!_core.IsPinyin) // 手写时退格
            {
                if (btn.Key == Keys.Back)
                    KeyboardInput.SendKey(pressedModifierKeyCodes, btn.KeyCode);
            }
        }

        /// <summary>
        /// 清空候选词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        /// <summary>
        /// 查看更多候选词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMore_Click(object sender, RoutedEventArgs e)
        {
            gridHandList.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Clipboard.SetDataObject(button.Content, true);
            SendKeys.SendWait("^v");
            Clear();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            gridHandList.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 清空集合中的元素
        /// </summary>
        private void Clear()
        {
            borderSelect.Visibility = Visibility.Hidden;
            gridHandList.Visibility = Visibility.Hidden;
            inkCanvas.Strokes.Clear();
            _core.ClearAlternates();
            _core.Keys = string.Empty;
        }

        /// <summary>
        /// 查找子节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private List<T> FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            try
            {
                List<T> list = new List<T>();
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child is T t)
                    {
                        list.Add(t);
                        List<T> childOfChildren = FindVisualChild<T>(child);
                        if (childOfChildren != null)
                        {
                            list.AddRange(childOfChildren);
                        }
                    }
                    else
                    {
                        List<T> childOfChildren = FindVisualChild<T>(child);
                        if (childOfChildren != null)
                        {
                            list.AddRange(childOfChildren);
                        }
                    }
                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 候选词条
        /// </summary>
        private void ShowCharatar()
        {
            borderSelect.Visibility = Visibility.Visible;
            string[] strs = CacheHelper.Get(_core.Keys);
            if (strs != null)
            {
                if (strs.Length > 7)
                {
                    _core.AlternatesMore = strs;
                    btnMore.IsEnabled = true;
                }
                else
                {
                    btnMore.IsEnabled = false;
                }
                string[] arr = strs.ToList().Take(7).ToArray();
                _core.Alternates = arr;
            }
        }

        #region Windows API

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        #endregion

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
