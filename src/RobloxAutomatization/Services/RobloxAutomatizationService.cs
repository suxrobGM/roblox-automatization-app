using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using RobloxAutomatization.Models;

namespace RobloxAutomatization.Services
{
    public class RobloxAutomatizationService : IRobloxAutomatizationService
    {
        private readonly ChromeOptions _chromeOptions;
        private IWebDriver _driver;

        public RobloxAutomatizationService()
        {
            _chromeOptions = new ChromeOptions();
            _chromeOptions.AddArgument("user-data-dir=robloxAutomatization");
        }

        public void ConfigureAntichatExtension()
        {
            _driver = new ChromeDriver(_chromeOptions);
            _driver.Navigate().GoToUrl("https://antcpt.com/eng/download/google-chrome-options/manual-crx.html");
        }

        public void OpenChrome()
        {
            _driver = new ChromeDriver(_chromeOptions);
            _driver.Navigate().GoToUrl("https://www.roblox.com");
        }

        public void RegisterNewUser(ref RobloxUser user, bool isCustomUsername = false)
        {
            if (IsElementPresent(By.XPath("//a[@data-bind='popover-setting']")))
            {
                Logout();
            }

            WaitForReady(By.Id("signup-button"));

            var dateSplitted = user.Birthday.ToString("MMM dd yyyy", new CultureInfo("en-US")).Split();
            var month = dateSplitted[0];
            var day = dateSplitted[1];
            var year = dateSplitted[2];

            //MessageBox.Show($"{month} {day} {year}");
            Thread.Sleep(1000);
            
            SelectOption(month, By.Id("MonthDropdown"));
            SelectOption(day, By.Id("DayDropdown"));
            SelectOption(year, By.Id("YearDropdown"));

            var usernameField = _driver.FindElement(By.Id("signup-username"));
            usernameField.SendKeys(user.Username);

            _driver.FindElement(By.Id("signup-password")).SendKeys(user.Password);

            if (user.Gender == Gender.Female)            
                _driver.FindElement(By.Id("FemaleButton")).Click();            
            else
                _driver.FindElement(By.Id("MaleButton")).Click();

            Thread.Sleep(1500);
            var errorMsg = _driver.FindElement(By.Id("signup-usernameInputValidation")).Text;
            while (!string.IsNullOrWhiteSpace(errorMsg))
            {
                if (isCustomUsername)
                {
                    user.Username = UserGenerator.AttachRandomNumber(user.Username);
                }
                else
                {
                    user.Username = UserGenerator.GenerateOnlyUsername();
                }
                
                usernameField.Clear();
                usernameField.SendKeys(user.Username);
                errorMsg = _driver.FindElement(By.Id("signup-usernameInputValidation")).Text;
            }
            Thread.Sleep(500);
            _driver.FindElement(By.Id("signup-button")).Click();
        }

        public void SaveUserCookies(string username)
        {
            if (!Directory.Exists("cookies"))            
                Directory.CreateDirectory("cookies");

            WaitForReady(By.Id("places-list-container"));

            var cookies = _driver.Manage().Cookies.AllCookies;
            File.WriteAllText($"cookies\\{username}.json", JsonConvert.SerializeObject(cookies, Formatting.Indented));
            Logout();
        }

        private void Logout()
        {
            _driver.FindElement(By.XPath("//a[@data-bind='popover-setting']")).Click();
            _driver.FindElement(By.XPath("//a[@data-behavior='logout']")).Click();
            _driver.Navigate().GoToUrl("https://www.roblox.com");
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
