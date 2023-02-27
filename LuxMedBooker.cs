using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuppeteerSharp;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace LuxmedBooker.Function
{
    public class LuxMedBooker
    {
        [FunctionName("LuxMedBooker")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            using var browserFetcher = new BrowserFetcher();
            var fetcher = await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            // string chromiumSavePath = fetcher.FolderPath.ToString(); for sake of debugging / cleaning files

            Browser browser = (Browser)await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            // Create a new page and go to Bing Maps
            Page page = (Page)await browser.NewPageAsync();
            await page.GoToAsync("https://rezerwacja.luxmed.pl/start/portalpacjenta");

            //login activities
            await page.TypeAsync("input[name='Login']", Environment.GetEnvironmentVariable("login"));
            await page.TypeAsync("input[name='Password']", Environment.GetEnvironmentVariable("password"));
            await page.ClickAsync("button[type='submit']");
            await page.WaitForNavigationAsync();

            //go to book visit
            await page.WaitForResponseAsync(response => response.Status.ToString() == "200");
            await page.ClickAsync("button.schedule-visit");
            await page.WaitForNavigationAsync();

            string content = await page.GetContentAsync();
            await browser.CloseAsync();

            return new OkObjectResult(content);
        }

        private List<string> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var programmerLinks = htmlDoc.DocumentNode.SelectNodes("//tr/td[@id='menu']/a").ToList();

            List<string> wikiLink = new List<string>();

            foreach (var link in programmerLinks)
            {
                wikiLink.Add(link.Attributes["href"].Value.ToString());
            }

            return wikiLink;
        }
    }
}
