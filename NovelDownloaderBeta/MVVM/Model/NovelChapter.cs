using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NovelDownloaderBeta.MVVM.Model
{
    public class NovelChapter
    {
        //constructor
        public NovelChapter(string title="title", string url="url")
        {
            this._title = title;
            this._url = url;
        }

        public override string ToString()
        {
            return $"Title: {this._title} Url= {this._url}";
        }

        //properties
        public string Title 
        {
            get 
            {
                return this._title;
            } 
            set
            {
                this._title = value.Trim();
            }
        }

        public string Url
        {
            get
            {
                return this._url;
            }
            set
            {
                this._url = value.Trim();
            }
        }


        //members
        private string _title;
        private string _url;
      
    }
}
