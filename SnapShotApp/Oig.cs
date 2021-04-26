using OpenQA.Selenium;
using System;

namespace SnapShotApp
{
	class Oig
	{
		private IWebDriver _driver;
		public Oig(Parser parse, ScreenShot screenShot, VerifyList verifyList)
		{
			InitDriver initDriver = new InitDriver();
			_driver = initDriver.Initialize("https://exclusions.oig.hhs.gov");
			//add OIG header to the first line in the verify list
			verifyList.AddToVerifyList("----- https://exclusions.oig.hhs.gov -----");
			foreach (var person in parse.PeopleList)
			{
				Console.WriteLine("[OIG]: " + person.LastName + ", " + person.FirstName);                                       //logging. Ensures the user knows something is happening
				_driver.Url = "https://exclusions.oig.hhs.gov";                                                                 //visit OIG exclusions website
				_driver.FindElement(By.Id("ctl00_cpExclusions_txtSPLastName")).SendKeys(person.LastName);                       //enter last name
				_driver.FindElement(By.Id("ctl00_cpExclusions_txtSPFirstName")).SendKeys(person.FirstName);                     //enter first name
				System.Threading.Thread.Sleep(100);                                                                             //wait a moment for typing to finish
				_driver.FindElement(By.Id("ctl00_cpExclusions_ibSearchSP")).Click();                                            //search for name
				screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - (OIG)", false, false);    //run screenshot of results
				Verify(person, screenShot, verifyList);                                                                                              //take screenshots of details
			}
			_driver.Quit();
		}

		private void Verify(Person person, ScreenShot screenShot, VerifyList verifyList)
		{
			bool CheckForDOB = (person.DateOfBirth != null);    //if DOB found, can automatically search page for match
			bool verifyDisplayed = _driver.PageSource.Contains("SSN/EIN");
			if (verifyDisplayed)
			{
				if (CheckForDOB)
				{
					verifyList.AddToVerifyList("[" + person.LastName + ", " + person.FirstName + "]: DOB Given - Verifying automatically. Will state if DOB was found.");
				}
				else
				{
					verifyList.AddToVerifyList("[" + person.LastName + ", " + person.FirstName + "]: No DOB Given - Check screenshots to verify");
				}
				for (int i = 2; i < 10; i++)    //click and screenshot up to 8 verify buttons
				{
					if (_driver.PageSource.Contains("ctl00_cpExclusions_gvEmployees_ctl0" + i + "_cmdVerify2"))
					{
						_driver.FindElement(By.Id("ctl00_cpExclusions_gvEmployees_ctl0" + i + "_cmdVerify2")).Click();
						//take a picture
						screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - VERIFY_" + (i - 1) + " - (OIG)", true, false);
						//if DOB found, log screenshot DOB was found in
						if (CheckForDOB)
						{
							if (_driver.PageSource.Contains(person.DateOfBirth))
							{
								verifyList.AddToVerifyList("[" + person.LastName + ", " + person.FirstName + "]:" + " DOB found in screenshot " + (i - 1));
							}
						}
						_driver.Url = "https://exclusions.oig.hhs.gov/SearchResults.aspx";
					}
					else
					{
						break;
					}
				}
				_driver.Url = "https://exclusions.oig.hhs.gov";
			}
		}
	}
}