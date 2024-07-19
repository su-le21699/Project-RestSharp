using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DataHelper;
using Core.Utilities;
using DemoQA.Service.Model.DataObject;

namespace DemoQA.Test.DataProvider
{
    public class UserProvider
    {
        static UserProvider()
        {
            DataProvider<UserDto>.Initialize("TestData\\Users\\User.json");
        }
        public static UserDto LoadUserDataByKey(string key)
        {
            return DataProvider<UserDto>.LoadDataByKey(key);
        }
    }
}