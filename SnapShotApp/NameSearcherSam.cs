using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

/**
*   This class searches https://www.sam.gov website
*   using the names parsed from the CSV file selected. After a search is performed
*   a screenshot is taken and saved to a folder with today's date.
*   TODO: fix scrolling for SAM
*   TODO: Automatically search for identifiable user info
**/

namespace SnapShotApp
{
    internal class NameSearcherSam
    {
        private IWebDriver _driver;

        public void SearchName(ScreenShot screenShot, Parser parse, string url, VerifyList verifyList, List<ExcludedPerson> excludedPersons)
        {
            //add OIG header to the first line in the verify list
            verifyList.AddToVerifyList("----- https://www.sam.gov -----"); Initialize(url);
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
                screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - (SAM)", false, "SAM");     //run screenshot of found names
                ClickViewDetails(screenShot, person, verifyList);                                                               //run screenshot of details
            }
            _driver.Quit();
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

        private void ClickViewDetails(ScreenShot screenShot, Person person, VerifyList verifyList)
        {
            //if more details on page
            if (_driver.PageSource.Contains("View Details"))
            {
                //I don't think this does anything
                person.Verified = true;
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
                        + " - Details - " + numberOfIterations + " - " + " (SAM)", true, "SAM");
                    //return to verification page
                    _driver.FindElement(By.XPath("//*[contains(@title, 'Return to Search')]")).Click();
                } 
            }
        }
        
    }
}