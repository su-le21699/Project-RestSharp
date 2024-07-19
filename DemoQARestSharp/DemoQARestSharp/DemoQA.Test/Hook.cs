using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Configuration;
using Core.Reports;
using Core.ShareData;
using Microsoft.Extensions.Configuration;

namespace DemoQA.Test
{
    [SetUpFixture]
    public class Hook
    {
        public static IConfiguration Config;
        const string AppSettings = "appsettings.json";

        [OneTimeSetUp]
        public void OneTimeStartup()
        {
            ConfigurationHelper.ReadConfiguration(AppSettings);
            DataStorage.InitData();
        }
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ExtentReportManager.GenerateReport();
        }
    }
}