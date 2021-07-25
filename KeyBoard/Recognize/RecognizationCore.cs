using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Ink;

namespace KeyBoard.Recognize
{
    /// <summary>
    /// 识别核心类
    /// </summary>
    public class RecognizationCore : INotifyPropertyChanged
    {
        #region SelectedRecognizer

        private ICharactorRecognizer _selectedRecoginzer;

        /// <summary>
        /// Get/Set 识别器
        /// </summary>
        public ICharactorRecognizer SelectedRecognizer
        {
            get => _selectedRecoginzer;
            set
            {
                if (_selectedRecoginzer != value)
                {
                    _selectedRecoginzer = value;
                    Alternates = Constants.EmptyAlternates;
                    NotifyPropertyChanged(nameof(SelectedRecognizer));
                }
            }
        }

        #endregion

        #region Alternates

        private IEnumerable<string> _alternates;

        /// <summary>
        /// Get 候选词列表
        /// </summary>
        public IEnumerable<string> Alternates
        {
            get => _alternates;
            set
            {
                if (_alternates != value)
                {
                    _alternates = value;
                    NotifyPropertyChanged(nameof(Alternates));
                }
            }
        }

        private IEnumerable<string> _alternatesMore;

        /// <summary>
        /// Get 候选词列表(更多)
        /// </summary>
        public IEnumerable<string> AlternatesMore
        {
            get => _alternatesMore;
            set
            {
                if (_alternatesMore != value)
                {
                    _alternatesMore = value;
                    NotifyPropertyChanged(nameof(AlternatesMore));
                }
            }
        }

        private bool _isPinyin = true;
        /// <summary>
        /// 是否拼音输入
        /// </summary>
        public bool IsPinyin
        {
            get => _isPinyin;
            set
            {
                if (_isPinyin != value)
                {
                    _isPinyin = value;
                    NotifyPropertyChanged(nameof(IsPinyin));
                }
            }
        }

        private string _keys;
        /// <summary>
        /// 拼音组合
        /// </summary>
        public string Keys
        {
            get => _keys;
            set
            {
                if (_keys != value)
                {
                    _keys = value;
                    NotifyPropertyChanged(nameof(Keys));
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged PropertyChanged

        /// <summary>
        /// 属性变化时触发
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// 创建一个识别核心类
        /// </summary>
        public RecognizationCore()
        {
            SelectedRecognizer = new ImprovedRecognizer();
        }

        /// <summary>
        /// 进行识别
        /// </summary>
        /// <param name="strokes">笔迹集合</param>
        public bool Recognize(StrokeCollection strokes)
        {
            if (SelectedRecognizer == null)
                return false;

            string[] strs = SelectedRecognizer.Recognize(strokes);
            Alternates = strs.TakeWhile((str) => str.Length == 1);

            return Alternates.Count() != 0;
        }

        /// <summary>
        /// 清空候选词
        /// </summary>
        public void ClearAlternates()
        {
            Alternates = Constants.EmptyAlternates;
        }
    }
}
