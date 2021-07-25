using System;
using System.IO;
using System.Runtime.Caching;
using System.Windows.Forms;

namespace KeyBoard.Helper
{
    public static class CacheHelper
    {
        static readonly MemoryCache _pinyinCache = new MemoryCache("JianTi");

        static CacheHelper()
        {
            string path = Application.StartupPath + @"\Resources\JianTi.dll";
            string[] arr = File.ReadAllLines(path);
            foreach (string item in arr)
            {
                string key = item.Substring(0, item.IndexOf(" "));
                string value = item.Substring(item.IndexOf(" ") + 1);
                _pinyinCache.Add(key, value, DateTimeOffset.MaxValue);
            }
        }

        public static string[] Get(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string str = string.Empty;

                try
                {
                    if (_pinyinCache.Contains(key))
                        str += " " + _pinyinCache[key].ToString();
                }
                catch { }

                if (!string.IsNullOrEmpty(str))
                {
                    string[] arr = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (arr[i].IndexOf("*") > -1)
                        {
                            arr[i] = arr[i].Substring(0, arr[i].IndexOf("*"));
                        }
                    }
                    return arr;
                }
            }
            return null;
        }
    }
}
