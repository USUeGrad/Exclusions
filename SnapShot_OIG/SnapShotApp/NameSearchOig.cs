using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

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
        private Actions _builder;

        public void SearchName(ScreenShot screenShot, Parser parse, string url, VerifyList verifyList, List<ExcludedPerson> excludedPersons)
        {
            Initilization(url);
            //add OIG header to the first line in the verify list
            verifyList.AddToVerifyList("----- https://exclusions.oig.hhs.gov -----");
            foreach (var person in parse.PeopleList)
            {
                BackToNameSearch();
                PageReadyCheck.CheckIfPageIsReady(ref _driver);
                //try {
                    InputLastName(person.LastName);
                    InputFirstName(person.FirstName);
                    System.Threading.Thread.Sleep(100);
                    Search();
                    //PerformActions();
                    PageReadyCheck.CheckIfPageIsReady(ref _driver);
                    screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - (OIG)", false, "OIG");
                    Verify(person, screenShot, url, verifyList);
                    System.Threading.Thread.Sleep(250);
                    //VerifyAgainstUpdatedList(verifyList, person, excludedPersons);
               // }
             //   catch (Exception)
             //   {
            //        BackToNameSearch();
             //   }
            }
            CloseDriver();
        }

        private static void VerifyAgainstUpdatedList(VerifyList verifyList, Person person, List<ExcludedPerson> excludedPersons)
        {
            var personFound = excludedPersons.FirstOrDefault(p => p.FirstName.ToLower() == person.FirstName.ToLower() &&
                                                p.LastName.ToLower() == person.LastName.ToLower() &&
                                                p.DateOfBirth == person.DateOfBirth);
            if (personFound != null) return;
            var personName = person.LastName + ", " + person.FirstName;
            verifyList.RemoveFromVerifyList(personName);
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
            _driver.FindElement(By.Id("ctl00_cpExclusions_txtSPLastName")).SendKeys(lName);
        }

        private void InputFirstName(string fName)
        {
            _driver.FindElement(By.Id("ctl00_cpExclusions_txtSPFirstName")).SendKeys(fName);
        }

        private void Search()
        {
            _driver.FindElement(By.Id("ctl00_cpExclusions_ibSearchSP")).Click();
        }

        private void PerformActions()
        {
            _builder.Perform();
        }

        private void Verify(Person person, ScreenShot screenShot, string url, VerifyList verifyList)
        {
            var verifyDisplayed = _driver.PageSource.Contains("SSN/EIN");
            if (verifyDisplayed)
            {
                person.Verified = true;
                //add person to verify to the verify list
                verifyList.AddToVerifyList(person.LastName + ", " + person.FirstName);
                VerifyPerson(person, screenShot);
                Console.Write("Verify: " + person.LastName + "," + person.FirstName + "\n");
                CloseDriver(); //close the driver (browser window)
                Initilization(url); //re-initialize driver and open a new browser window
            }
        }

        private void VerifyPerson(Person person, ScreenShot screenShot)
        {
            //id tag for initial verify buttons start at 2, hence i = 2 on initilization
            //will not work for 1 >= 10 due to how verify searching works

            for (var i = 2; i < 10; i++)
            {
                if (InitialVerifyButtonIsVisible(i))
                {
                    ScreenshotEachVerify(i, screenShot, person);
                    //VerifyPersonBySocial(person);
                    //IsVerifyDone();
                    BackToVerifySearchResults();
                    PageReadyCheck.CheckIfPageIsReady(ref _driver);
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

        private void ScreenshotEachVerify(int i, ScreenShot screenShot, Person person)
        {
            //program clicks each verify button and takes a picture. Once all verify buttons have been clicked, returns to previous menu
            try {
                _driver.FindElement(By.Id("ctl00_cpExclusions_gvEmployees_ctl0" + i + "_cmdVerify2")).Click();
            }
            catch(Exception) {
                BackToNameSearch();
            }
                PageReadyCheck.CheckIfPageIsReady(ref _driver);

            try
            {
                _driver.FindElement(By.XPath("//*[contains(text(),person.DateOfBirth)]"));

                    screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - VERIFY_" + i + " - (OIG)", true, "OIG");
            }
            catch (Exception) {
                System.Windows.Forms.MessageBox.Show("Could not find text: " + person.DateOfBirth);
            }

        }
        //currently not being used, but might be in the future
        private void VerifyPersonBySocial(Person person)
        {
            do
            {
                InputSocial(SocialPopUpBox(person));
                ClickVerifyButton();
                PageReadyCheck.CheckIfPageIsReady(ref _driver);
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
            var strResponse = Microsoft.VisualBasic.Interaction.InputBox(
                            "Please Enter the SS# (without dashes) for " + 
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

        private bool IncorrectSocialFormat()
        {
            var verify = _driver.PageSource.Contains("ctl00_cpExclusions_lblValidate");
            return verify;
        }

        private void BackToVerifySearchResults()
        {
            _driver.Navigate().GoToUrl("https://exclusions.oig.hhs.gov/SearchResults.aspx");
            PageReadyCheck.CheckIfPageIsReady(ref _driver);
        }

        private void BackToNameSearch()
        {
            _driver.Navigate().GoToUrl("https://exclusions.oig.hhs.gov");
        }

        private void CloseDriver()
        {
            _driver.Close();
        }
    }
}