using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using AngleSharp;
using AngleSharp.Dom;


namespace NovelDownloaderBeta.MVVM.Model
{
    
    public class FreeWebNovelDownloader : NovelDownloader
    {
        //CONSTRUCTOR
        public FreeWebNovelDownloader(string url) : base(url)
        {
            //pass url to base constructor
        }

        public override async Task GetNovelInfo()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(this.NovelUrl);

            this.NovelTitle = document.QuerySelector("h3.tit")?.TextContent ?? String.Empty;
            this.NovelAuthor = document.QuerySelector("span.s1")?.TextContent ?? String.Empty;
            this.CoverUrl=document.QuerySelector("div.m-imgtxt")?.QuerySelector("img")?.GetAttribute("src") ?? String.Empty;

            CreateDownloadFolder();
            await GetChapterUrls(document);

            ChapterCount = _novelChapters.Count;
        }

        private List<string> GetChapterPages(IDocument document)
        {
            List<string> chapterPages = new List<string>();

            var pages = document.QuerySelector("div.page");


            var links = pages?.QuerySelectorAll("option");

            if (links == null)
                return chapterPages;

            foreach (var link in links)
            {
                string? url = link?.GetAttribute("value");

                if (url != null)
                    chapterPages.Add(url);
            }

            return chapterPages;
        }
        protected override async Task GetChapterUrls(IDocument document)
        {
            List<string> pages = await Task.Run<List<string>>(() => { return GetChapterPages(document); });

            foreach (string url in pages)
            {
                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                using var doc = await context.OpenAsync(FreeWebNovelDownloader._websiteUrl+url);

                var chapterList = doc.QuerySelector("div.m-newest2");
                var chapterLinks = chapterList?.QuerySelectorAll("a").Where(link => link.GetAttribute("title") != null);

                if (chapterLinks == null)
                    continue;

                foreach(var link in chapterLinks)
                {
                    string title = link?.GetAttribute("title") ?? "chapter";
                    string? chapterUrl = link?.GetAttribute("href");

                    if (chapterUrl != null)
                        this._novelChapters.Add(new NovelChapter(title, chapterUrl));
                }
            }

        }

        public override async Task DownloadNovel()
        {
            this.Status = Status.DOWNLOADING;
            await GetCoverImage();
            string chapterFolder = DownloadFolder + $"\\Chapters";
            Directory.CreateDirectory(chapterFolder);
            
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            foreach (var chapter in _novelChapters)
            {
                var name = chapterFolder+"\\"+string.Format("page{0}.xhtml", Progress + 1);
                using var doc = await context.OpenAsync(FreeWebNovelDownloader._websiteUrl + chapter.Url);
                string? chapterText = doc.QuerySelector("div.txt ")?.InnerHtml;
                StreamWriter chaprerWriter;

                using (chaprerWriter= new StreamWriter(name))
                {
                    await chaprerWriter.WriteLineAsync(_pageTemplate.Replace("{0}", chapter.Title).Replace("{1}", chapterText));
                }
                
                Progress++;

            }

            GenerateEpub();

            this.Status = Status.COMPLETE;
        }

    

        
        protected override void GenerateEpub()
        {
            var epub = new net.vieapps.Components.Utility.Epub.Document();

            epub.AddBookIdentifier("0000");
            epub.AddLanguage("English");
            epub.AddTitle(NovelTitle);
            epub.AddAuthor(NovelAuthor);
            epub.AddPublisher(FreeWebNovelDownloader._websiteUrl);
            epub.AddMetaItem("dc:contributor", "contributor");
            epub.AddMetaItem("book:Original", "original");
            //epub.AddStylesheetData("style.css", styleSheet);

            //cover
            epub.AddImageFile(DownloadFolder+ "\\cover.jpg", "cover.jpg");
            epub.AddXhtmlData("page0.xhtml",_pageTemplate.Replace("{0}","Cover").Replace("{1}", "<img src=\"cover.jpg\" alt=\"Cover\"/>"));

            for(int i=1; i<=Progress; i++)
            {
                epub.AddXhtmlFile(DownloadFolder + "\\Chapters" + $"\\page{i}.xhtml", $"page{i}.xhtml");
                epub.AddNavPoint("Chapter "+ i, $"page{i}.xhtml", i);
            }


            epub.Generate(DownloadFolder + $"\\{NovelTitle}.epub");

        }
        
        private const string _websiteUrl = "https://freewebnovel.com";
    }

    
}