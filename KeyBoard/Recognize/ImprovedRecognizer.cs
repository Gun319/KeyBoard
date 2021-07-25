using Microsoft.Ink;
using System.Collections.Generic;
using System.IO;
using System.Windows.Ink;

namespace KeyBoard.Recognize
{
    /// <summary>
    /// 识别器
    /// </summary>
    public class ImprovedRecognizer : ICharactorRecognizer
    {
        /// <summary>
        /// 识别
        /// </summary>
        /// <param name="strokes">笔迹集合</param>
        /// <returns>候选词数组</returns>
        public string[] Recognize(StrokeCollection strokes)
        {
            if (strokes == null || strokes.Count == 0)
                return Constants.EmptyAlternates;

            RecognitionResult theRecognitionResult;
            // 识别文字列表
            List<string> wordList = new List<string>();

            MemoryStream savedInk = new MemoryStream();
            strokes.Save(savedInk);
            Ink theInk = new Ink();
            theInk.Load(savedInk.ToArray());

            // 创建墨迹识别器上下文
            RecognizerContext theRecognizerContext = new RecognizerContext
            {
                Strokes = theInk.Strokes
            };

            // 对墨迹收集器中当前的笔划执行识别
            theRecognitionResult = theRecognizerContext.Recognize(out RecognitionStatus theRecognitionStatus);

            // 获取顶级备选方案的行备选方案集合
            // 最多100项
            RecognitionAlternates theLineAlternates = theRecognitionResult.GetAlternatesFromSelection(0, -1, 100);

            foreach (RecognitionAlternate theAlternate in theLineAlternates)
            {
                wordList.Add(theAlternate.ToString());
            }
            return wordList.ToArray();
        }
    }
}
