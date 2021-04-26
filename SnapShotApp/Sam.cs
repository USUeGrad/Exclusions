using OpenQA.Selenium;
using System.Collections.Generic;

namespace SnapShotApp
{
	class Sam
	{
		private IWebDriver _driver;
		public Sam(Parser parse, ScreenShot screenShot, VerifyList verifyList)
		{
			//add SAM header to the first line in the verify list
			verifyList.AddToVerifyList("----- https://www.sam.gov -----");
			InitDriver initDriver = new InitDriver();
			_driver = initDriver.Initialize("https://www.sam.gov");
			foreach (var person in parse.PeopleList)
			{
				System.Console.WriteLine("[SAM]: " + person.LastName + ", " + person.FirstName);                                //logging. Ensures the user knows something is happening
				_driver.FindElement(By.Id("searchForm:search")).Click();                                                        //go to search records page
				_driver.FindElement(By.Id("searchBasicForm:qterm_input")).SendKeys(person.LastName + ", " + person.FirstName);  //enter name
				System.Threading.Thread.Sleep(100);                                                                             //wait a moment for typing to finish
				_driver.FindElement(By.Id("searchBasicForm:SearchButton")).Click();                                             //search for name
				_driver.FindElement(By.Id("searchResultsID:PIRCheckBox")).Click();                                              //filter to exclusion reports
				_driver.FindElement(By.XPath("//*[contains(@title, 'Apply Filters')]")).Click();                                //apply filter
				System.Threading.Thread.Sleep(100);                                                                             //wait a moment
				_driver.FindElement(By.XPath("//*[contains(@title, 'Select to Continue')]")).Click();                           //confirm
				screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - (SAM)", false, true);     //run screenshot of found names
				Verify(person, screenShot, verifyList);                                                                         //run screenshot of details
			}
			_driver.Quit();
		}

		private void Verify(Person person, ScreenShot screenShot, VerifyList verifyList)
		{
			//if more details on page
			if (_driver.PageSource.Contains("View Details"))
			{
				//add person to verify to the verify list
				verifyList.AddToVerifyList(person.LastName + ", " + person.FirstName);
				//generate list of options to click
				IList<IWebElement> options = _driver.FindElements(By.XPath("//input[starts-with(@title, 'View Details')]"));
				//for each member in list
				for (int numberOfIterations = 0; numberOfIterations < options.Count; numberOfIterations++)
				{
					//regenerate list
					options = _driver.FindElements(By.XPath("//input[starts-with(@title, 'View Details')]"));
					//System.Threading.Thread.Sleep(100);
					//click next option in list
					options[numberOfIterations].Click();
					//run screenshot
					screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - VERIFY - "
							+ " - Details - " + numberOfIterations + " - " + " (SAM)", true, true);
					//return to verification page
					_driver.FindElement(By.XPath("//*[contains(@title, 'Return to Search')]")).Click();
				}
			}
		}
	}
}
