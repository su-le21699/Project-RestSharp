using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;

namespace Core.DataHelper
{
    public class DataProvider<T>
    {
        private static Dictionary<string, T> _data;

        public static void Initialize(string filePath)
        {
            _data = JsonUtils.ReadDictionaryJson<T>(filePath);
        }
        public static T LoadDataByKey(string key)
        {
            return _data[key];
        }
    }
}
