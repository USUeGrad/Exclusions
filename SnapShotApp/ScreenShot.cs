using OpenQA.Selenium;

/**
*   This class performs a screenshot of the browser. This includes scrolling, resizing, etc. to ensure a high-quality image.   
**/

namespace SnapShotApp
{
    internal class ScreenShot
    {
        private static ITakesScreenshot _screenshotHandler;
        private static string _folderDestination;

        public ScreenShot(string folderDestination)
        {
            _folderDestination = folderDestination;
        }

        private static void TakeScreenShot(ref IWebDriver driver, string filename, bool verified, string webSiteName)
        {
            _screenshotHandler = driver as ITakesScreenshot;
            if (verified)
            {
                if (webSiteName == "SAM")
                {
                    ZoomInPageForScreenShot(ref driver);
                    ScrollPageVerticalForScreenShot(ref driver, 400);
                }
                if (_screenshotHandler != null)
                {
                    var screenshot = _screenshotHandler.GetScreenshot();
                    screenshot.SaveAsFile(_folderDestination + filename + ".png", ScreenshotImageFormat.Png);
                }
                ZoomOutPageForScreenShot(ref driver);
            }
            else
            {
                if (_screenshotHandler != null)
                {
                    var screenshot = _screenshotHandler.GetScreenshot();
                    screenshot.SaveAsFile(_folderDestination + filename + ".png", ScreenshotImageFormat.Png);
                }
            }
        }

        public void RunScreenShot(ref IWebDriver driver, string filename, bool verified, string webSiteName)
        {
            var currentDate = System.DateTime.Now.ToString("MM_dd_yyyy");
            TakeScreenShot(ref driver, filename + " - " + currentDate, verified, webSiteName);
        }

        private static void ZoomInPageForScreenShot(ref IWebDriver driver)
        {
            var executor = (IJavaScriptExecutor) driver;
            executor.ExecuteScript("document.body.style.zoom='75%'");
        }
        private static void ZoomOutPageForScreenShot(ref IWebDriver driver)
        {
            var executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("document.body.style.zoom='100%'");
        }
        private static void ScrollPageVerticalForScreenShot(ref IWebDriver driver, int amountToScroll)
        {
            var executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("window.scrollBy(0,250)", "");
        }
    }
}