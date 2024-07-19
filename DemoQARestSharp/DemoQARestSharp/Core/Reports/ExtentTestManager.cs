using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using Core.Utilities;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;

namespace Core.Reports
{
    public class ExtentTestManager
    {
        private static AsyncLocal<ExtentTest> _parentTest = new AsyncLocal<ExtentTest>();
        private static AsyncLocal<ExtentTest> _childTest = new AsyncLocal<ExtentTest>();

        public static ExtentTest CreateParentTest(string testName, string description = null)
        {
            _parentTest.Value = ExtentReportManager.Instance.CreateTest(testName, description);
            return _parentTest.Value;
        }

        public static ExtentTest CreateTest(string testName, string description = null)
        {
            if (_parentTest.Value == null)
            {
                throw new InvalidOperationException("Parent test is not set. Ensure CreateParentTest is called before CreateTest.");
            }
            _childTest.Value = _parentTest.Value.CreateNode(testName, description);
            return _childTest.Value;
        }

        public static ExtentTest GetTest()
        {
            if (_childTest.Value == null)
            {
                throw new InvalidOperationException("Child test is not set. Ensure CreateTest is called before GetTest.");
            }
            return _childTest.Value;
        }
        public static void LogTestOutcome(TestContext context, IWebDriver driver = null)
        {
            var outcome = context.Result.Outcome.Status;
            var stackTrace = string.IsNullOrEmpty(context.Result.StackTrace)
                ? ""
                : string.Format("<pre>{0}</pre>", context.Result.StackTrace);
            Status logStatus;
            var className = context.Test.ClassName;
            var testName = context.Test.Name;
            switch (outcome)
            {
                case TestStatus.Failed:
                    logStatus = Status.Fail;
                    if (driver is not null)
                    {
                        var fileLocation = ScreenshotHelper.CaptureScreenshot(driver, className, testName);
                        testName = FileUtils.SanitizeFileName(testName);
                        var mediaEntity = ScreenshotHelper.CaptureScreenShotAndAttachToExtendReport(fileLocation);
                        ReportLog.Fail($"#Test Name:  {testName}  #Status:  {logStatus} {stackTrace}", mediaEntity);
                    }
                    else ReportLog.Fail($"#Test Name:  {testName}  #Status:  {logStatus} {stackTrace}");
                    // ReportLog.Fail("#Screenshot Below: " + ReportLog.AddScreenCaptureFromPath(fileLocation));
                    break;
                case TestStatus.Inconclusive:
                    logStatus = Status.Warning;
                    ReportLog.Skip("#Test Name: " + testName + " #Status: " + logStatus);
                    break;
                case TestStatus.Skipped:
                    logStatus = Status.Skip;
                    ReportLog.Skip("#Test Name: " + testName + " #Status: " + logStatus);
                    break;
                default:
                    logStatus = Status.Pass;
                    ReportLog.Pass("#Test Name: " + testName + " #Status: " + logStatus);
                    break;
            }
            // GetTest().Log(logStatus, "Test ended with " + logStatus + stackTrace);
        }
    }
}