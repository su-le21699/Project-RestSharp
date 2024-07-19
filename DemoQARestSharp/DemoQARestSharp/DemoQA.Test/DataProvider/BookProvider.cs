using Core.DataHelper;
using Core.Utilities;
using DemoQA.Service.Model.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoQA.Test.DataProvider
{
    public class BookProvider
    {
        static BookProvider()
        {
            DataProvider<BookDto>.Initialize("TestData\\Book\\Book.json");
        }
        public static BookDto LoadBookDataByKey(string key)
        {
            return DataProvider<BookDto>.LoadDataByKey(key);
        }
    }
}
