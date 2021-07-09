using OpenQA.Selenium;

namespace SnapShotApp
{
	class Sam
	{
		private IWebDriver _driver;
		public Sam(Parser parse, ScreenShot screenShot, VerifyList verifyList)
		{
			//add SAM header to the first line in the verify list
			verifyList.AddToVerifyList("----- https://sam.gov -----");
			//initialize driver and visit advanced exclusions search page
			InitDriver initDriver = new InitDriver();
			_driver = initDriver.Initialize("https://sam.gov/search?index=ex&sfm[status][is_active]=true");
			//deal with modal that appears when page is first visited
			if (_driver.PageSource.Contains("SAM.gov has merged with beta.SAM.gov."))
			{
				_driver.FindElement(By.XPath("//*[contains(text(), 'OK')]")).Click();
			}

			foreach (var person in parse.PeopleList)
			{
				//logging
				System.Console.WriteLine("[SAM]: " + person.LastName + ", " + person.FirstName);
				//go to advanced search page
				_driver.Url = "https://sam.gov/search?index=ex&sfm[status][is_active]=true";
				//add Excluded Individual to options. For some reason, it's the only option off by default.
				_driver.FindElement(By.XPath("//*[contains(text(), 'More Filters')]")).Click();
				//selenium can't seem to deal with elements in a modal - using js instead
				var js = (IJavaScriptExecutor)_driver;
				js.ExecuteScript("document.getElementById('formly_54_checkbox_exclusionClassificationIndividualWrapper_2').click();");
				js.ExecuteScript("document.getElementsByClassName('usa-button')[10].click();");
				//wait for animations to complete
				System.Threading.Thread.Sleep(300);
				_driver.FindElement(By.XPath("//*[contains(text(), 'Excluded Individual')]")).Click(); 
				_driver.FindElement(By.Id("firstname")).SendKeys(person.FirstName);
				_driver.FindElement(By.Id("lastname")).SendKeys(person.LastName);
				_driver.FindElement(By.XPath("//*[contains(text(), 'Filter By Individual')]")).Click();
				//wait for data to render
				System.Threading.Thread.Sleep(500);
				screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - (SAM)");                                                                   
			}
			_driver.Quit();
		}
	}
}
