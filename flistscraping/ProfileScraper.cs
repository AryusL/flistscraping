using System;
using System.Collections.Generic;
using System.Xml.Linq;
using flistscraping;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace flistscraping
{
    public class ProfileScraping
    {
        private readonly string url;
        private readonly HtmlDocument htmlDoc;
        private CharacterInfo characterInfo;
        public CharacterInfo CharacterInfo { get { return characterInfo; } }

        public ProfileScraping(string url)
        {
            this.url = url;
            characterInfo = new CharacterInfo();
            HtmlWeb web = new HtmlWeb();
            htmlDoc = web.Load(url);
            // Select the <a> tag using HtmlAgilityPack
            var anchorTag = htmlDoc.DocumentNode.SelectSingleNode("//a[@id='SplashWarningYes']");

            // Check if the tag exists
            if (anchorTag != null)
            {
                // Create a WebBrowser control and navigate to the link URL
                // create a ChromeDriver instance
                var options = new ChromeOptions();
                options.AddArgument("--headless"); // run in headless mode (no browser window)
                var driver = new ChromeDriver(options);

                // navigate to the initial page
                driver.Navigate().GoToUrl(url);

                // find the link to click
                var link = driver.FindElement(By.Id("SplashWarningYes"));

                // click on the link
                link.Click();

                // get the HTML of the current page
                var html = driver.PageSource;

                htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                // dispose the driver
                driver.Quit();
            }
        }

        public void ScrapeCharacterInfoSideBar()
        {
            // Find the Character_InfoBox div and scrape its content
            var characterInfoBox = htmlDoc.DocumentNode.SelectSingleNode("//td[@id='Sidebar']");
            if (characterInfoBox != null)
            {
                // Scrape the Name variable from the span with the charname class
                string name = characterInfoBox.SelectSingleNode(".//span[@class='charname']")?.InnerText;
                characterInfo.SetName(name);
                // Scrape the additional variables from the statbox div
                var statBox = characterInfoBox.SelectSingleNode(".//div[@class='statbox']");
                if (statBox != null)
                {
                    // Loop through each span tag within the statbox div
                    foreach (var span in statBox.SelectNodes(".//span"))
                    {
                        // Get the name of the variable from the span tag's inner text
                        string variableName = span.InnerText;

                        // Get the value of the variable from the span tag's next sibling (which is a text node)
                        string variableValue = span.NextSibling?.InnerText?.Trim();
                        if (!String.IsNullOrEmpty(variableValue) && variableValue.Length > 2)
                        {
                            variableValue = variableValue.Substring(2);

                            // Add the variable to the dictionary
                            characterInfo.AddSideBarInfo(variableName, variableValue);
                        }
                    }
                }
            }
        }

        public void ScrapeCharacterRpInfo()
        {
            // Find the Character_InfoBox div and scrape its content
            var tabsRpInfo = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='tabs-2']");
            if (tabsRpInfo != null)
            {
                // Loop through each span tag within the statbox div
                foreach (var span in tabsRpInfo.SelectNodes(".//span[@class='taglabel']"))
                {
                    // Get the name of the variable from the span tag's inner text
                    string variableName = span.InnerText;
                    if (!String.IsNullOrEmpty(variableName))
                    {
                        variableName = variableName.Substring(0, variableName.Length - 1);

                        // Get the value of the variable from the span tag's next sibling (which is a text node)
                        string variableValue = span.NextSibling?.InnerText?.Trim();
                        if (!String.IsNullOrEmpty(variableValue))
                        {
                            // Add the variable to the dictionary
                            characterInfo.AddRpInfo(variableName, variableValue);
                        }
                    }
                }
            }
        }

        public void ScrapeCharacterKink()
        {
            // Find the Character_InfoBox div and scrape its content
            var kinksTable = htmlDoc.DocumentNode.SelectSingleNode("//table[@id='Character_FetishList']");
            if (kinksTable != null)
            {
                var faveKinksTd = kinksTable.SelectSingleNode("//td[@id='Character_FetishlistFave']");
                ScrapeKinks(faveKinksTd, CharacterInfo.KinkPosition.FAVE);
                var yesKinksTd = kinksTable.SelectSingleNode("//td[@id='Character_FetishlistYes']");
                ScrapeKinks(yesKinksTd, CharacterInfo.KinkPosition.YES);
                var maybeKinksTd = kinksTable.SelectSingleNode("//td[@id='Character_FetishlistMaybe']");
                ScrapeKinks(maybeKinksTd, CharacterInfo.KinkPosition.MAYBE);
            }
        }

        public void ScrapeKinks(HtmlNode node, CharacterInfo.KinkPosition kp)
        {
            foreach (var a in node.SelectNodes(".//a"))
            {
                bool isCustom = a.HasClass("Character_CustomFetish");
                var content = a.InnerHtml;

                if (isCustom)
                    characterInfo.AddCustomKink(content, kp);
                else
                    characterInfo.AddKink(content, kp);
            }
        }
    }
}
