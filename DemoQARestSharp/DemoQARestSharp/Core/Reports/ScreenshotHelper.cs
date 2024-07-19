using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using OpenQA.Selenium;

namespace Core.Reports
{
    public class ScreenshotHelper
    {
        public static string CaptureScreenshot(IWebDriver driver, string className, string testName)
        {
            try
            {
                ITakesScreenshot ts = (ITakesScreenshot)driver;
                Screenshot screenshot = ts.GetScreenshot();
                var screenshotDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots", className);
                testName = testName.Replace("\"", "");
                var fileName = string.Format(@"Screenshot_{0}_{1}", testName, DateTime.Now.ToString("yyyyMMdd_HHmmssff"));
                Directory.CreateDirectory(screenshotDirectory);
                var fileLocation = string.Format(@"{0}\{1}.png", screenshotDirectory, fileName);
                screenshot.SaveAsFile(fileLocation);
                return fileLocation;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in CaptureScreenshot: " + ex.Message);
                throw;
            }
        }
        public static MediaEntityModelProvider CaptureScreenShotAndAttachToExtendReport(string path)
        {
            return MediaEntityBuilder.CreateScreenCaptureFromPath(path).Build();
        }
    }
}