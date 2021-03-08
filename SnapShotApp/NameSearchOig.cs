using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium.Interactions;

/**
*   This class searches https://exclusions.oig.hhs.gov website
*   using the names parsed from the CSV file selected. After a search is performed
*   a screenshot is taken and saved to a folder dated with today's date.
*   
*   Important: https://exclusions.oig.hhs.gov only allows 2 consecutive
*   "non-matching" SSN verifys - the third one will not work. Therefore,
*   I have programmed the Chrome driver to close and re-open after each
*   'person' is verified. This will need to be adjusted if there are more
*   than 2 SSN verifys on one 'person'.
**/

namespace SnapShotApp
{
    internal class NameSearchOig
    {
        private IWebDriver _driver;

        public void SearchName(ScreenShot screenShot, Parser parse, string url, VerifyList verifyList, List<ExcludedPerson> excludedPersons)
        {
            Initialize(url);
            //add OIG header to the first line in the verify list
            verifyList.AddToVerifyList("----- https://exclusions.oig.hhs.gov -----");
            foreach (var person in parse.PeopleList)
            {
                Console.WriteLine("[OIG]: " + person.LastName + ", " + person.FirstName);                   //logging. Makes sure the user knows something is happening
                _driver.Url = "https://exclusions.oig.hhs.gov";                                             //visit OIG exclusions website
                _driver.FindElement(By.Id("ctl00_cpExclusions_txtSPLastName")).SendKeys(person.LastName);   //enter last name
                _driver.FindElement(By.Id("ctl00_cpExclusions_txtSPFirstName")).SendKeys(person.FirstName); //enter first name
                System.Threading.Thread.Sleep(100);                                                         //wait a moment for typing to finish
                _driver.FindElement(By.Id("ctl00_cpExclusions_ibSearchSP")).Click();                        //search for name
                screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - (OIG)", false, "OIG"); //run screenshot of results
                Verify(person, screenShot, url, verifyList);                                                //take screenshots of details
                //System.Threading.Thread.Sleep(250);
            }
            _driver.Close();
        }

        private void Initialize(string url)
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

            _driver = new ChromeDriver(chromeDriverService, chromeOptions);              //create driver with above arguments
            _driver.Url = url;                                      //visit given url
        }

        private void Verify(Person person, ScreenShot screenShot, string url, VerifyList verifyList)
        {
            var verifyDisplayed = _driver.PageSource.Contains("SSN/EIN");
            if (verifyDisplayed)
            {
                person.Verified = true;
                //add person to verify to the verify list
                VerifyPerson(person, screenShot, verifyList);
                _driver.Url = url;
            }
        }

        private void VerifyPerson(Person person, ScreenShot screenShot, VerifyList verifyList)
        {
            //id tag for initial verify buttons start at 2, hence i = 2 on initilization
            //will not work for 1 >= 10 due to how verify searching works
            bool CheckForDOB = (person.DateOfBirth != null);    //if DOB found, can automatically search page for match
            if (CheckForDOB)
            {
                verifyList.AddToVerifyList("[" + person.LastName + ", " + person.FirstName + "]: DOB Given - Verifying automatically. Will state if DOB was found.");
            }
            else
            {
                verifyList.AddToVerifyList("[" + person.LastName + ", " + person.FirstName + "]: No DOB Given - Check screenshots to verify");
            }
            for (var i = 2; i < 10; i++)    //click and screenshot each verify button
            {
                if (_driver.PageSource.Contains("ctl00_cpExclusions_gvEmployees_ctl0" + i + "_cmdVerify2"))
                {
                    ScreenshotEachVerify(i, screenShot, person, CheckForDOB, verifyList);
                    _driver.Url = "https://exclusions.oig.hhs.gov/SearchResults.aspx";
                }
                else
                {
                    return;
                }
            }
        }
        

        private void ScreenshotEachVerify(int i, ScreenShot screenShot, Person person, bool CheckForDOB, VerifyList verifyList)
        {
            //click verify button
            _driver.FindElement(By.Id("ctl00_cpExclusions_gvEmployees_ctl0" + i + "_cmdVerify2")).Click();
            //take a picture
            screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - VERIFY_" + (i-1) + " - (OIG)", true, "OIG");
            //if DOB found, log screenshot DOB was found in
            if (CheckForDOB) {
                if (_driver.PageSource.Contains(person.DateOfBirth)) {
                    verifyList.AddToVerifyList("[" + person.LastName + ", " + person.FirstName +  "]:" + " DOB found in screenshot " + (i-1));
                }
            }
        }
        
        
        
    }
}