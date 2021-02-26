using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

/**
*   This class searches https://www.sam.gov website
*   using the names parsed from the CSV file selected. After a search is performed
*   a screenshot is taken and saved to a folder with today's date.
*   
**/

namespace SnapShotApp
{
    internal class NameSearcherSam
    {
        private IWebDriver _driver;
        private Actions _builder;

        public void SearchName(ScreenShot screenShot, Parser parse, string url, VerifyList verifyList, List<ExcludedPerson> excludedPersons)
        {
            //add OIG header to the first line in the verify list
            verifyList.AddToVerifyList("----- https://www.sam.gov -----"); Initilization(url);
            PageReadyCheck.CheckIfPageIsReady(ref _driver);
            foreach (var person in parse.PeopleList)
            {
               // try
                //{
                    ClickSearchRecordsButton();
                    PageReadyCheck.CheckIfPageIsReady(ref _driver);
                    InputName(person.LastName, person.FirstName);
                    System.Threading.Thread.Sleep(100);                  //sleep for 0.1 seconds - program moves too quickly for its own good
                    Search();
                    //PageReadyCheck.CheckIfPageIsReady(ref _driver);
                    ExclusionCheckBox();
                    ClickApplyFiltersButton();
                    ClickContinue();
                    PageReadyCheck.CheckIfPageIsReady(ref _driver);
                    screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - (SAM)", false, "SAM");
                    ClickViewDetails(screenShot, person, verifyList);
                    //VerifyAgainstUpdatedList(verifyList, person, excludedPersons);
               // }
                //catch (Exception){}
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

        private void ClickSearchRecordsButton()
        {
            PageReadyCheck.CheckIfPageIsReady(ref _driver);
            _driver.FindElement(By.Id("searchForm:search")).Click();
        }

        private void InputName(string lName, string fName)
        {
            PageReadyCheck.CheckIfPageIsReady(ref _driver);
            _driver.FindElement(By.Id("searchBasicForm:qterm_input")).SendKeys(lName + ", " + fName);
        
        }

        private void Search()
        {
            PageReadyCheck.CheckIfPageIsReady(ref _driver);
            _driver.FindElement(By.Id("searchBasicForm:SearchButton")).Click();
        }

        private void ExclusionCheckBox()
        {
            PageReadyCheck.CheckIfPageIsReady(ref _driver);
            _driver.FindElement(By.Id("searchResultsID:PIRCheckBox")).Click();
        }

        private void ClickApplyFiltersButton()
        {
            PageReadyCheck.CheckIfPageIsReady(ref _driver);

            /* Program will crash if this value is not correct. This can be found by right-clicking the "apply filters" button and clicking on "inspect" */
            //_driver.FindElement(By.Name("searchResultsID:j_idt161")).Click();

            _driver.FindElement(By.XPath("//*[contains(@title, 'Apply Filters')]")).Click();
        }

        private void ClickContinue()
        {
            System.Threading.Thread.Sleep(1000);
            //var continueButton = _driver.FindElement(By.XPath("//button[contains(@title, 'Select to Continue')]"));
            //_builder.MoveToElement(continueButton).Click().Perform();
            _driver.FindElement(By.XPath("//*[contains(@title, 'Select to Continue')]")).Click();
        }

        private bool IsViewDetailsPresent()
        {
            PageReadyCheck.CheckIfPageIsReady(ref _driver);
            return _driver.PageSource.Contains("View Details");
        }

        private void ClickViewDetails(ScreenShot screenShot, Person person, VerifyList verifyList)
        {
            if (IsViewDetailsPresent())
            {
                person.Verified = true;
                //add person to verify to the verify list
                verifyList.AddToVerifyList(person.LastName + ", " + person.FirstName);
                IList<IWebElement> options; //list of elements to iterate through
                options = _driver.FindElements(By.XPath("//input[starts-with(@title, 'View Details')]"));
                for (int numberOfIterations = 0; numberOfIterations < options.Count; numberOfIterations++)
                {
                    options = _driver.FindElements(By.XPath("//input[starts-with(@title, 'View Details')]"));
                    System.Threading.Thread.Sleep(100);
                    options[numberOfIterations].Click();
                    screenShot.RunScreenShot(ref _driver, person.LastName + "_" + person.FirstName + " - VERIFY - "
                        + " - Details - " + numberOfIterations + " - " + " (SAM)", true, "SAM");
                    ReturnToSearchClick();
                } 
            }
        }

        private void ReturnToSearchClick()
        {
            //var returnToSearchButton = _driver.FindElement(By.XPath("//input[contains(@title, 'Return to Search')]"));
            //_builder.MoveToElement(returnToSearchButton).Click().Perform();
            _driver.FindElement(By.XPath("//*[contains(@title, 'Return to Search')]")).Click();
            PageReadyCheck.CheckIfPageIsReady(ref _driver);
        }

        private void CloseDriver()
        {
            _driver.Close();
        }
    }
}