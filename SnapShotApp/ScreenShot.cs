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

		public void RunScreenShot(ref IWebDriver driver, string filename, bool verified, bool isSAM)
		{
			var filenameDate = filename + "-" + System.DateTime.Now.ToString("MM_dd_yyyy");
			var executor = (IJavaScriptExecutor)driver;
			_screenshotHandler = driver as ITakesScreenshot;
			if (_screenshotHandler != null)
			{
				if (verified)
				{
					if (isSAM)
					{
						//executor.ExecuteScript("document.body.style.zoom='125%'");
						executor.ExecuteScript("window.scrollBy(0,600)");
						System.Threading.Thread.Sleep(300);                         //additional info takes 1/4 second to unfold :/
						executor.ExecuteScript("window.scrollBy(0,600)");
					}
					var screenshot = _screenshotHandler.GetScreenshot();
					screenshot.SaveAsFile(_folderDestination + filenameDate + ".png", ScreenshotImageFormat.Png);
					executor.ExecuteScript("document.body.style.zoom='100%'");
				}
				else
				{
					var screenshot = _screenshotHandler.GetScreenshot();
					screenshot.SaveAsFile(_folderDestination + filenameDate + ".png", ScreenshotImageFormat.Png);
				}
			}
		}
	}
}