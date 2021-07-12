using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Windows.Forms;

namespace SnapShotApp
{
	class InitDriver
	{
		private IWebDriver _driver;

		public IWebDriver Initialize(string url)
		{

			var chromeOptions = new ChromeOptions();                //default chrome arguments
			chromeOptions.AddArgument("window-size=1920,1080");     //full screen
			//disable following arguments for bugfixing
			chromeOptions.AddArgument("headless");                  //invisible browser
			chromeOptions.AddArgument("disable-gpu");               //not rendering anything, so gpu disabled
			chromeOptions.AddArgument("hide-scrollbars");           //scrollbars hidden
			chromeOptions.AddArgument("log-level=3");               //hide jquery warnings

			var chromeDriverService = ChromeDriverService.CreateDefaultService();   //additional options
			chromeDriverService.HideCommandPromptWindow = true;                     //hide chromedriver command window.
			chromeDriverService.SuppressInitialDiagnosticInformation = true;        //hide chromedriver setup info

			try
			{
				_driver = new ChromeDriver(chromeDriverService, chromeOptions) { Url = url };   //create driver with above arguments
			}
			catch (InvalidOperationException)
			{
				MessageBox.Show("Chromedriver out of date. To fix: \n" +
						"1. Run CleanChrome.bat from Data folder.\n" +
						"2. Download latest ChromeDriver file from chromedriver.chromium.org and put it in Data folder.");
				_driver.Quit();
				System.Environment.Exit(0);
			}
			catch (OpenQA.Selenium.WebDriverException)
			{
				MessageBox.Show("Chrome out of date. Update Chrome by going to Chrome > Settings > About Chrome.");
				_driver.Quit();
				System.Environment.Exit(0);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: " + e);
				_driver.Quit();
				System.Environment.Exit(0);
			}
			_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
			return _driver;
		}
	}
}
