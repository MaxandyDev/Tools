using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;

namespace AI_Image_Agents.Helper
{
    public class LoginAccount
    {
        private string email;
        private string password;
        private IWebDriver driver;

        public LoginAccount(string email, string password)
        {
            this.email = email;
            this.password = password;
            this.driver = new ChromeDriver();
        }

        public void Login(string url, string emailSelector, string continueBtn, string passwordSelector, string loginBtn)
        {
            driver.Navigate().GoToUrl(url);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.FindElement(By.Id(emailSelector)).SendKeys(email);
            driver.FindElement(By.ClassName(continueBtn)).Click();

            driver.FindElement(By.Id(passwordSelector)).SendKeys(password);
            driver.FindElement(By.ClassName(loginBtn)).Click();
        }

        public void Quit()
        {
            driver.Quit();
        }
    }
}
