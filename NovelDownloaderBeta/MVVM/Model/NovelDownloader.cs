using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using NovelDownloaderBeta.Core;

namespace NovelDownloaderBeta.MVVM.Model
{
    public enum Status { QUEUED, DOWNLOADING, COMPLETE }
    public abstract class NovelDownloader: ObservableObject
    {
        //CONSTRUCTOR
        public NovelDownloader(string url)
        {
            this.NovelUrl = url.Trim();
            this._novelChapters = new List<NovelChapter>();
            this._pageTemplate = @"<?xml version='1.0' encoding='utf-8'?>
<!DOCTYPE html>
<html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:epub=""http://www.idpf.org/2007/ops"" epub:prefix=""z3998: http://www.daisy.org/z3998/2012/vocab/structure/#"" lang=""en"" xml:lang=""en"">
  <head>
    <title>{0}</title>
  </head>
  <body dir=""default"">{1}</body>
</html>".Trim().Replace("\t", "");

            _status=Status.QUEUED;
        }

        //Methods
        protected abstract void GenerateEpub();
        public abstract Task DownloadNovel();
        public abstract  Task GetNovelInfo();
        protected abstract Task GetChapterUrls(AngleSharp.Dom.IDocument document);
        protected async Task<string> GetCoverImage()
        {
            if (CoverUrl == null)
                return String.Empty;

            string fileName = DownloadFolder + "\\cover.jpg";
            HttpClient httpClient = new HttpClient();
            Stream imageStream = await httpClient.GetStreamAsync(this.CoverUrl);
            Bitmap bitmap = new Bitmap(imageStream);


            using(httpClient)
            {
                using(imageStream)
                {
                    if (bitmap != null)
                    {
                        bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

            return fileName;
        }
        protected string CreateDownloadFolder()
        {
            string directory = "Downloads\\" + this.NovelTitle;
            Directory.CreateDirectory(directory);
            DownloadFolder = directory;
            return directory;
        }


        //properties
        public string NovelUrl { get; init; }
        public string CoverUrl { get; set; }
        public string NovelTitle 
        {
            get 
            {
                return _novelTitle;
            }
            set 
            {
                _novelTitle = value;
                OnPropertyChanged();
            }
        }
        public string NovelAuthor { get; set; }
        public int ChapterCount
        {
            get
            {
                return _chapterCount;
            }
            set
            {
                _chapterCount = value;
                OnPropertyChanged();
            }
        }
        public string DownloadFolder{ get; set; }
        public int Progress 
        {
            get 
            {
                return this._progress; 
            }
            set
            {
                this._progress = value;
                OnPropertyChanged();
            }
        }
        public Status Status 
        { 
            get 
            {
                return this._status;
            } 
            set
            {
                this._status = value;
                OnPropertyChanged();
            } 
        }


        //member variables
        protected List<NovelChapter> _novelChapters;
        protected string _pageTemplate;
        protected int _progress;
        protected Status _status;
        protected int _chapterCount;
        protected string _novelTitle;
    }



   
}
