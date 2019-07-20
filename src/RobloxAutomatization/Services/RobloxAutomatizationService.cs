using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using RobloxAutomatization.Models;

namespace RobloxAutomatization.Services
{
    public class RobloxAutomatizationService
    {
        private IWebDriver _driver;

        public RobloxAutomatizationService()
        {
        }
        
        public void OpenChrome()
        {
            var driverService = ChromeDriverService.CreateDefaultService();
            var chromeOptions = new ChromeOptions();
            _driver = new ChromeDriver(driverService, chromeOptions);
        }

        public void GoToRobloxSite()
        {
            _driver.Navigate().GoToUrl("https://www.roblox.com/account/signupredir");
        }

        public void RegisterNewUser(ref RobloxUser user)
        {
            WaitForReady(By.Id("signup-button"));

            var dateSplitted = user.Birthday.ToString("MMM dd yyyy", new CultureInfo("en-US")).Split();
            var month = dateSplitted[0];
            var day = dateSplitted[1];
            var year = dateSplitted[2];
            SelectOption(month, By.Id("MonthDropdown"));
            SelectOption(day, By.Id("DayDropdown"));
            SelectOption(year, By.Id("YearDropdown"));
            _driver.FindElement(By.Id("signup-username")).SendKeys(user.Username);

            var errorMsg = _driver.FindElement(By.Id("signup-usernameInputValidation")).Text;
            while (!string.IsNullOrWhiteSpace(errorMsg))
            {
                user.Username = UserGenerator.GenerateOnlyUsername();
                _driver.FindElement(By.Id("signup-username")).SendKeys(user.Username);
                errorMsg = _driver.FindElement(By.Id("signup-usernameInputValidation")).Text;
            }

            _driver.FindElement(By.Id("signup-password")).SendKeys(user.Password);

            if (user.Gender == Gender.Female)            
                _driver.FindElement(By.Id("FemaleButton")).Click();            
            else
                _driver.FindElement(By.Id("MaleButton")).Click();

            _driver.FindElement(By.Id("signup-button")).Click();
        }

        public void SaveUserCookies(string username)
        {
            if (!Directory.Exists("cookies"))            
                Directory.CreateDirectory("cookies");

            WaitForReady(By.Id("places-list-container"));

            var cookies = _driver.Manage().Cookies.AllCookies;
            File.WriteAllText($"cookies\\{username}.json", JsonConvert.SerializeObject(cookies, Formatting.Indented));            
        }

        private void SelectOption(string optionName, By selectInput)
        {
            optionName = optionName.Trim();
            var selectElement = _driver.FindElement(selectInput);
            selectElement.Click();

            if (IsElementPresent(By.XPath($".//option[contains(text(), '{optionName}')]")))
            {
                selectElement.FindElement(By.XPath($".//option[contains(text(), '{optionName}')]")).Click();
            }
            else if (IsElementPresent(By.XPath($".//option[contains(text(), '{optionName.ToUpper()}')]")))
            {
                selectElement.FindElement(By.XPath($".//option[contains(text(), '{optionName.ToUpper()}')]")).Click();
            }
            else if (IsElementPresent(By.XPath($".//option[contains(text(), '{optionName.ToLower()}')]")))
            {
                selectElement.FindElement(By.XPath($".//option[contains(text(), '{optionName.ToLower()}')]")).Click();
            }
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                _driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private void WaitForReady(By by)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromHours(2));
            wait.Until(driver =>
            {
                //bool isAjaxFinished = (bool)((IJavaScriptExecutor)driver).ExecuteScript("return jQuery.active == 0");
                return IsElementPresent(by);
            });
        }
    }
}
