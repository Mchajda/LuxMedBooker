using System;
using PuppeteerSharp;
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace LuxmedBooker.Interfaces {
    public interface IPageManipulator {

        public  void InitPuppeteer();
        public  void GoToAsync(string url);
        public  void TypeAsync(string fieldName, string input);
        public  void ClickByTypeAsync(string type);
        public  Task<string> GetPageConstentAsync();
        public  void CloseBrowserAsync();
    }
}