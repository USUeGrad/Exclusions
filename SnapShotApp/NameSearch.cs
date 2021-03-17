using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SnapShotApp
{
    class NameSearch
    {
        private IWebDriver _driver;
        private Parser _parse;
        private ScreenShot _screenShot;
        private VerifyList _verifyList;

        public NameSearch(string file, string folderDestination)
        {
            _parse = new Parser();
            _parse.Parse(file);
            _screenShot = new ScreenShot(folderDestination);
            _verifyList = new VerifyList();
        }

        public void CountNames()
        {
            _verifyList.AddToVerifyList("Total number of names checked: " + (_parse.counter - 1));
        }

        public void SearchOig()
        {
            Initialize("https://exclusions.oig.hhs.gov");
            //add OIG header to the first line in the verify list
            _verifyList.AddToVerifyList("----- https://exclusions.oig.hhs.gov -----");
            foreach (var person in _parse.PeopleList)
            {
                Console.WriteLine("[OIG]: " + person.LastName + ", " + person.FirstName);                                       //logging. Ensures the user knows something is happening
                _driver.Url = "https://exclusions.oig.hhs.gov";                                                                 //visit OIG exclusions website
                _driver.FindElement(By.Id("ctl00_cpExclusions_txtSPLastName")).SendKeys(person.LastName);                       //enter last name
                _driver.FindElement(By.Id("ctl00_cpExclusions_txtSPFirstName")).SendKeys(person.FirstName);                     //enter first name
                System.Threading.Thread.Sleep(100);                                                                             //wait a moment for typing to finish
                _driver.FindElement(By.Id("ctl00_cpExclusions_ibSearchSP")).Click();                                            //search for name
                _screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - (OIG)", false, "OIG");    //run screenshot of results
                OIGVerify(person);                                                                                              //take screenshots of details
            }
            _driver.Quit();
        }

        public void SearchSam()
        {
            //add OIG header to the first line in the verify list
            _verifyList.AddToVerifyList("----- https://www.sam.gov -----");
            Initialize("https://www.sam.gov");
            foreach (var person in _parse.PeopleList)
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
                _screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - (SAM)", false, "SAM");    //run screenshot of found names
                SAMVerify(person);                                                                                              //run screenshot of details
            }
            _driver.Quit();
        }

        public void WriteSummary(string folderLocation)
        {
            using (var file = new System.IO.StreamWriter(folderLocation + "!! SUMMARY !!.txt"))
            {
                foreach (var line in _verifyList.ReturnVerifyList())
                {
                    file.WriteLine(line);
                }
            }
            MessageBox.Show("Screenshots Complete!", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

            try
            {
                _driver = new ChromeDriver(chromeDriverService, chromeOptions) {Url = url};   //create driver with above arguments
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Chromedriver out of date. To fix: \n" +
                    "1. Run CleanChrome.bat from Data folder.\n" +
                    "2. Download latest ChromeDriver file from chromedriver.chromium.org and put it in Data folder.");
                System.Environment.Exit(0);
            }
            catch (OpenQA.Selenium.WebDriverException)
            {
                MessageBox.Show("Chrome out of date. Update Chrome by going to Chrome > Settings > About Chrome.");
                System.Environment.Exit(0);
            }
        }

        private void OIGVerify(Person person)
        {
            bool CheckForDOB = (person.DateOfBirth != null);    //if DOB found, can automatically search page for match
            bool verifyDisplayed = _driver.PageSource.Contains("SSN/EIN");
            if (verifyDisplayed)
            {
                if (CheckForDOB)
                {
                    _verifyList.AddToVerifyList("[" + person.LastName + ", " + person.FirstName + "]: DOB Given - Verifying automatically. Will state if DOB was found.");
                }
                else
                {
                    _verifyList.AddToVerifyList("[" + person.LastName + ", " + person.FirstName + "]: No DOB Given - Check screenshots to verify");
                }
                for (int i = 2; i < 10; i++)    //click and screenshot up to 8 verify buttons
                {
                    if (_driver.PageSource.Contains("ctl00_cpExclusions_gvEmployees_ctl0" + i + "_cmdVerify2"))
                    {
                        _driver.FindElement(By.Id("ctl00_cpExclusions_gvEmployees_ctl0" + i + "_cmdVerify2")).Click();
                        //take a picture
                        _screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - VERIFY_" + (i - 1) + " - (OIG)", true, "OIG");
                        //if DOB found, log screenshot DOB was found in
                        if (CheckForDOB)
                        {
                            if (_driver.PageSource.Contains(person.DateOfBirth))
                            {
                                _verifyList.AddToVerifyList("[" + person.LastName + ", " + person.FirstName + "]:" + " DOB found in screenshot " + (i - 1));
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

        private void SAMVerify(Person person)
        {
            //if more details on page
            if (_driver.PageSource.Contains("View Details"))
            {
                //add person to verify to the verify list
                _verifyList.AddToVerifyList(person.LastName + ", " + person.FirstName);
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
                    _screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - VERIFY - "
                        + " - Details - " + numberOfIterations + " - " + " (SAM)", true, "SAM");
                    //return to verification page
                    _driver.FindElement(By.XPath("//*[contains(@title, 'Return to Search')]")).Click();
                }
            }
        }
    }
}
