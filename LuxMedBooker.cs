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
using LuxmedBooker.Services;

namespace LuxmedBooker.Function
{
    public class LuxMedBooker
    {
        private PageManipulator _pageManipulator;
        public LuxMedBooker(PageManipulator pageManipulator)
        {
            _pageManipulator = pageManipulator;
        }

        [FunctionName("LuxMedBooker")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            _pageManipulator.InitPuppeteer();
            _pageManipulator.GoToAsync(Environment.GetEnvironmentVariable("url"));

            //login activities
            _pageManipulator.TypeAsync("Login", Environment.GetEnvironmentVariable("login"));
            _pageManipulator.TypeAsync("Password", Environment.GetEnvironmentVariable("password"));

            _pageManipulator.ClickByTypeAsync("submit");

            //go to book visit
            // await page.WaitForResponseAsync(response => response.Status.ToString() == "200");
            // await page.ClickAsync("button.schedule-visit");
            // await page.WaitForNavigationAsync();

            string content = await _pageManipulator.GetPageConstentAsync();
            _pageManipulator.CloseBrowserAsync();

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
