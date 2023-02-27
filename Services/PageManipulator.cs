using System;
using PuppeteerSharp;
using HtmlAgilityPack;
using System.Threading.Tasks;
using LuxmedBooker.Interfaces;

namespace LuxmedBooker.Services {
    public class PageManipulator : IPageManipulator {
        Browser _browser;
        private Page _page;

        public async void InitPuppeteer()
        {
            using var browserFetcher = new BrowserFetcher();
            var fetcher = await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            // string chromiumSavePath = fetcher.FolderPath.ToString(); for sake of debugging / cleaning files

            _browser = (Browser)await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            _page = (Page)await _browser.NewPageAsync();
        }

        public async void GoToAsync(string url)
        {
            await _page.GoToAsync(url);
        }

        public async void TypeAsync(string fieldName, string input)
        {
            await _page.TypeAsync($"input[name='{fieldName}']", input);
        }

        public async void ClickByTypeAsync(string type)
        {
            await _page.ClickAsync($"button[type='{type}']");
            await _page.WaitForNavigationAsync();
        }

        public async Task<string> GetPageConstentAsync()
        {
            return await _page.GetContentAsync();
        }

        public async void CloseBrowserAsync()
        {
            await _browser.CloseAsync();
        }
    }
}