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

namespace LuxmedBooker.Function
{
    public static class LuxMedBooker
    {
        [FunctionName("LuxMedBooker")]
        public static async Task<IActionResult> Run(
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
            await page.GoToAsync("https://mfo2.pl/start2/");

            await page.WaitForSelectorAsync("#auto_mf_login");
            await page.FocusAsync("#auto_mf_login");
            await page.Keyboard.TypeAsync("chajfox");

            await page.WaitForSelectorAsync("#auto_mf_password");
            await page.FocusAsync("#auto_mf_password");
            await page.Keyboard.TypeAsync("maciek911");

            var server = await page.WaitForSelectorAsync("#auto_mf_world_id");
            await page.SelectAsync("#auto_mf_world_id", "6");

            await page.ClickAsync(".guzik");
            await page.WaitForNavigationAsync();

            //credentials chajfox/maciek911

            // await page.ScreenshotAsync("C:\\Files\\image.png");
            string content = await page.GetContentAsync();
            await browser.CloseAsync();

            return new OkObjectResult(content);
        }
    }
}
