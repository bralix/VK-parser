using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Net;

namespace VKParser
{
    class Parser
    {
        public ChromeDriver chr;

        List<String> textOfFeeds = new List<string>();
        List<String> LinksOfFeeds = new List<string>();
        List<String> ImagesOfFeeds = new List<string>();

        Task ThreadOfText;
        Task ThreadOfImgLinks;
        Task ThreadOfLinks;

        public string jsonPathText = @"C:\JsonFiles\PostTexts.json";
        public string jsonPathLinks = @"C:\JsonFiles\PostLinks.json";
        public string jsonPathImgLinks = @"C:\JsonFiles\PostImgLinks.json";
        public static string jsonFolder = @"C:\JsonFiles\";

        public static bool isReady = false;
        public static string strToLoad = String.Empty;


        private void jsonTextLoader(List<IWebElement> feeds)
        {
            Dictionary<string, List<string>> textJS = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(jsonPathText));
            if (textJS == null)
            {
                textJS = new Dictionary<string, List<string>>();
            }
            foreach (IWebElement feed in feeds)
            {
                IReadOnlyCollection<IWebElement> textRowsCollection = feed.FindElements(By.ClassName("wall_post_text"));
                string postId = feed.GetAttribute("id");

                if (textRowsCollection.Count > 0)
                {
                    textOfFeeds = textRowsCollection.Select(el => el.Text).Where(text => (text.Length > 0)).ToList();
                    // It translates the list of elements into the text list of the elements

                    foreach (string text in textOfFeeds)
                    {

                        if (!textJS.ContainsKey(postId))
                        {
                            textJS.Add(postId, new string[] { text }.ToList());
                        }
                        else
                        {
                            if (!textJS[postId].Contains(text))
                                textJS[postId].Add(text);
                        }
                    }
                }
            }

            if (textJS.Count > 0)
            {
                File.Create(jsonPathText + ".lock").Close();
                File.WriteAllText(jsonPathText, JsonConvert.SerializeObject(textJS, Formatting.Indented));
                File.Delete(jsonPathText + ".lock");
            }
        }
        private void jsonImgLinksLoader(List<IWebElement> feeds)
        {
            Dictionary<string, List<string>> imagesJS = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(jsonPathImgLinks));

            if (imagesJS == null)
            {
                imagesJS = new Dictionary<string, List<string>>();
            }
            foreach (IWebElement feed in feeds)
            {
                IReadOnlyCollection<IWebElement> textRowsCollection = feed.FindElements(By.ClassName("wall_post_text"));
                string postId = feed.GetAttribute("id");

                IReadOnlyCollection<IWebElement> imageContainers = feed.FindElements(By.ClassName("page_post_sized_thumbs"));
                if (imageContainers.Count > 0)
                {
                    IWebElement imageContainer = imageContainers.ElementAt<IWebElement>(0);
                    IReadOnlyCollection<IWebElement> images = imageContainer.FindElements(By.ClassName("page_post_thumb_wrap"));

                    foreach (IWebElement image in images)
                    {
                        string styleText = image.GetAttribute("style");
                        Regex regex = new Regex("background-image: url\\(\"(.*?)\"\\);");
                        Match match = regex.Match(styleText);
                        if (match.Groups.Count > 0)
                        {
                            // if the link is not empty And the list of posts contains a post with the same ID And in this post there is no such link or there is no post with such an ID then we execute ...
                            if (
                                (imagesJS.ContainsKey(postId) == true &&
                                imagesJS[postId].Contains(match.Groups[1].Value) == false ||
                                imagesJS.ContainsKey(postId) == false)
                                )
                                if (!imagesJS.ContainsKey(postId))
                                    imagesJS.Add(postId,
                                        new string[] {
                                        match.Groups[1].Value
                                         }.ToList()
                                     );
                                else
                                    imagesJS[postId].Add(match.Groups[1].Value);
                        }
                    }
                }
            }

            if (imagesJS.Count > 0)
            {
                File.Create(jsonPathImgLinks + ".lock").Close();
                File.WriteAllText(jsonPathImgLinks, JsonConvert.SerializeObject(imagesJS, Formatting.Indented));
                File.Delete(jsonPathImgLinks + ".lock");
            }

        }
        private void jsonLinksLoader(List<IWebElement> feeds)
        {

            Dictionary<string, List<string>> linksJS = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(jsonPathLinks));

            if (linksJS == null)
            {
                linksJS = new Dictionary<string, List<string>>();
            }

            foreach (IWebElement feed in feeds)
            {
                IReadOnlyCollection<IWebElement> textRowsCollection = feed.FindElements(By.ClassName("wall_post_text"));
                string postId = feed.GetAttribute("id");

                if (textRowsCollection.Count > 0)
                {
                    List<IWebElement> linksContainer = textRowsCollection.ElementAt(0).FindElements(By.CssSelector("a")).ToList();

                    if (linksContainer.Count > 0)
                    {
                        foreach (IWebElement link in linksContainer)
                        {
                            if (
                                link.GetAttribute("href") != null &&
                                (linksJS.ContainsKey(postId) == true &&
                                linksJS[postId].Contains(link.GetAttribute("href")) == false ||
                                linksJS.ContainsKey(postId) == false)
                                )
                                if (!linksJS.ContainsKey(postId))
                                    linksJS.Add(postId,
                                        new string[] {
                                        link.GetAttribute("href")
                                        }.ToList()
                                    );
                                else
                                    linksJS[postId].Add(link.GetAttribute("href"));
                        }
                    }
                }
            }

            if (linksJS.Count > 0)
            {
                File.Create(jsonPathLinks + ".lock").Close();
                File.WriteAllText(jsonPathLinks, JsonConvert.SerializeObject(linksJS, Formatting.Indented));
                File.Delete(jsonPathLinks + ".lock");
            }
        }

        private void output()
        {
            Dictionary<string, List<string>> textJS = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(jsonPathText));
            Dictionary<string, List<string>> imagesJS = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(jsonPathImgLinks));
            Dictionary<string, List<string>> linksJS = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(jsonPathLinks));

            if (textJS == null)
                textJS = new Dictionary<string, List<string>>();
            if (imagesJS == null)
                imagesJS = new Dictionary<string, List<string>>();
            if (linksJS == null)
                linksJS = new Dictionary<string, List<string>>();

            int i = 0;
            foreach (string key in textJS.Keys)
            {
                i++;
                strToLoad += "<div class='feed'>\n" +
                                "\t<p>Post #" + i + ": </p>" + key + "<br>\n";

                foreach (string text in textJS[key])
                {
                    strToLoad += "\t<p>" + text.Replace("\r\n", "<br>") + "</p>" + "\n\t<br>\n\t<hr>\n";
                }
                if (linksJS.Keys.Contains(key))
                {
                    foreach (string link in linksJS[key])
                    {
                        strToLoad += "\t<a href='" + link + "'>Link</a>" + "\n\t<br>\n\t<hr>\n";
                    }
                }
                if (imagesJS.Keys.Contains(key))
                {
                    foreach (string image in imagesJS[key])
                    {
                        strToLoad += "\t<img src='" + image + "' class='feedImg'>" + "\n\t<br>\n\t<hr>\n";
                    }
                }
                strToLoad += "</div>\n";
            }

            isReady = true;
        }

        public void TaskVK(BackgroundWorker bw)
        {
            bw.ReportProgress(2);

            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            //Adds to the chrome command line an attribute that prevents it from loading images.

            chr = new OpenQA.Selenium.Chrome.ChromeDriver(chromeDriverService, options);

            bw.ReportProgress(25);

            chr.Manage().Window.Maximize();
            chr.Navigate().GoToUrl("https://vk.com");

            IWebElement fieldLogin = chr.FindElement(By.CssSelector("#index_email"));
            fieldLogin.SendKeys(DataStorage.Login);
            IWebElement fieldPassword = chr.FindElement(By.CssSelector("#index_pass"));
            fieldPassword.SendKeys(DataStorage.Password);
            chr.FindElement(By.CssSelector("#index_login_button")).Click();

            bw.ReportProgress(50);

            WebDriverWait webDriverWait = new WebDriverWait(chr, TimeSpan.FromSeconds(30));
            webDriverWait.Until(d => chr.Url.Equals("https://vk.com/feed"));

            bw.ReportProgress(70);

            List<IWebElement> feeds = chr.FindElements(By.ClassName("post")).ToList();

            ThreadOfText = new Task(() => { jsonTextLoader(feeds); });
            ThreadOfImgLinks = new Task(() => { jsonImgLinksLoader(feeds); });
            ThreadOfLinks = new Task(() => { jsonLinksLoader(feeds); });

            ThreadOfText.Start();
            ThreadOfImgLinks.Start();
            ThreadOfLinks.Start();

            ThreadOfText.Wait();
            ThreadOfImgLinks.Wait();
            ThreadOfLinks.Wait();

            output();
            chr.Quit();
            bw.ReportProgress(100);
        }
    }
}
