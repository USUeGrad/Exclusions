using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

/**
*   This class searches https://exclusions.oig.hhs.gov website
*   using the names parsed from the CSV file. After a search is performed
*   a screenshot is taken and saved to the 'screenshots' folder.
*   
*   Important: https://exclusions.oig.hhs.gov only allows 2 consecutive
*   "not matching" SSN verifys - the third one will not work. Therefore,
*   I have programmed the Chrome driver to close and re-open after each
*   'person' is verified. This will need to be adjusted if there are more
*   than 2 SSN verifys on one 'person'.
**/

namespace SnapShotApp
{
    internal class NameSearcher
    {
        private IWebDriver _driver;
        private Actions _builder;

        public void SearchName(ScreenShot screenShot, Parser parse, string url)
        {
            Initilization(url);
            foreach (var person in parse.PeopleList)
            {
                InputLastName(person.LastName);
                InputFirstName(person.FirstName);
                Search();
                PerformActions();
                Verify(person, screenShot, url);
                if (person.Verified == false)
                {
                    WaitTimer(1500);
                    screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName);
                }
                BackToNameSearch();
            }
            CloseDriver();
            Completed();
        }

        private void Initilization(string url)
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl(url);
            _builder = new Actions(_driver);
        }

        private void InputLastName(string lName)
        {
            var lastName = _driver.FindElement(By.XPath("//input[starts-with(@id, 'ctl00_cpExclusions_txtSPLastName')]"));
            _builder.MoveToElement(lastName).Click().SendKeys(lName);
        }

        private void InputFirstName(string fName)
        {
            var firstName = _driver.FindElement(By.XPath("//input[starts-with(@id, 'ctl00_cpExclusions_txtSPFirstName')]"));
            _builder.MoveToElement(firstName).Click().SendKeys(fName);
        }

        private void Search()
        {
            var search = _driver.FindElement(By.XPath("//input[starts-with(@title, 'Search')]"));
            _builder.MoveToElement(search).Click();
        }

        private void PerformActions()
        {
            _builder.Perform();
        }

        private void Verify(Person person, ScreenShot screenShot, string url)
        {
            var verifyDisplayed = _driver.PageSource.Contains("SSN/EIN");
            if (verifyDisplayed)
            {
                person.SetTag(true);
                VerifyPerson(person, screenShot);
                person.Verified = true;
                Console.Write("Verified: " + person.LastName + "," + person.FirstName + "\n");
                CloseDriver(); //close the driver (browser window)
                Initilization(url); //re-initialize driver and open a new browser window
            }
            else
            {
                person.SetTag(false);
            }
        }

        private void VerifyPerson(Person person, ScreenShot screenShot)
        {
            //id tag for initial verify buttons start at 2, hence i = 2 on initilization
            for (var i = 2; i < 50; i++)
            {
                if (InitialVerifyButtonIsVisible(i))
                {
                    ClickInitialVerifyButton(i);
                    VerifyPersonBySocial(person);
                    IsVerifyDone();
                    screenShot.RunScreenShot(ref _driver,
                        person.LastName + "_" + person.FirstName + "_VERIFIED_" + i + "_");
                    BackToVerifySearchResults();
                }
                else
                {
                    break;
                }
            }
        }

        private bool InitialVerifyButtonIsVisible(int i)
        {
            var verify = _driver.PageSource.Contains("ctl00_cpExclusions_gvEmployees_ctl0" + i + "_cmdVerify2");
            return verify;
        }

        private void ClickInitialVerifyButton(int i)
        {
            var initialVerifyButton = _driver.FindElement(By.XPath("//a[starts-with(@id, 'ctl00_cpExclusions_gvEmployees_ctl0" + i + "_cmdVerify2')]"));
            _builder.MoveToElement(initialVerifyButton).Click().Perform();
        }

        private void VerifyPersonBySocial(Person person)
        {
            do
            {
                InputSocial(SocialPopUpBox(person));
                ClickVerifyButton();
                WaitTimer(2000);
            } while (IncorrectSocialFormat());
        }

        private void InputSocial(string str)
        {
            var social = _driver.FindElement(By.XPath("//input[starts-with(@id, 'ctl00_cpExclusions_txtSSN')]"));
            social.Clear();
            _builder.MoveToElement(social).Click().SendKeys(str).Perform();
        }

        private static string SocialPopUpBox(Person person)
        {
            var strResponse = Interaction.InputBox(
                            "Plesae Enter the SS# (without dashes) for " + 
                            person.FirstName + 
                            " " + 
                            person.LastName,
                            "SSN Input");
            return strResponse;
        }

        private void ClickVerifyButton()
        {
            var socialButton = _driver.FindElement(By.XPath("//input[starts-with(@id, 'ctl00_cpExclusions_ibtnVerify')]"));
            _builder.MoveToElement(socialButton).Click().Perform();
        }

        private void IsVerifyDone()
        {
            new WebDriverWait(_driver, TimeSpan.FromSeconds(10)).Until(ExpectedConditions.ElementExists(
                By.XPath("//span[starts-with(@id, 'ctl00_cpExclusions_mayprint_lower')]")));
        }

        private bool IncorrectSocialFormat()
        {
            var verify = _driver.PageSource.Contains("ctl00_cpExclusions_lblValidate");
            return verify;
        }

        private void BackToVerifySearchResults()
        {
            _driver.Navigate().GoToUrl("https://exclusions.oig.hhs.gov/SearchResults.aspx");
        }

        private void BackToNameSearch()
        {
            _driver.Navigate().GoToUrl("https://exclusions.oig.hhs.gov");
        }

        private void CloseDriver()
        {
            _driver.Close();
        }

        private static void Completed()
        {
            MessageBox.Show("Screenshots Complete!", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private static void WaitTimer(int num)
        {
            Thread.Sleep(num);
        }
    }
}