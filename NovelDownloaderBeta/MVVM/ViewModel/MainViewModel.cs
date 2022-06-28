using NovelDownloaderBeta.Core;
using NovelDownloaderBeta.MVVM.Model;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;

namespace NovelDownloaderBeta.MVVM.ViewModel
{
    internal class MainViewModel:ObservableObject
    {
        private object _currentView;
        private string _urlTextBox;
        public DownloadsViewModel DownloadsView { get; set; }
        public SettingsViewModel SettingsView { get; set; }
        public AboutViewModel AboutView { get; set; }

        public RelayCommand OpenDownloadsView { get; set; }
        public RelayCommand OpenSettingsView { get; set; }
        public RelayCommand OpenAboutView { get; set; }

        public RelayCommand AddCommand { get; set; }

        public RelayCommand DownloadCommand { get; set; }   

        public ObservableCollection<NovelDownloader> CurrentDownloads { get; set; }
        public ObservableCollection<string> Names { get; set; }
        
        public object CurrentView
        {
            get { return _currentView; }
            set 
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public string UrlTextBox
        {
            get { return _urlTextBox; }
            set 
            { 
                if(!String.Equals(_urlTextBox,value))
                {
                    _urlTextBox = value;
                    OnPropertyChanged();
                }
            }
        }

        private NovelDownloader _selectedDownload;

        public NovelDownloader SelectedDownload
        {
            get { return _selectedDownload; }
            set { _selectedDownload = value; OnPropertyChanged(); }
        }


        public MainViewModel()
        {
            DownloadsView = new DownloadsViewModel();
            SettingsView=new SettingsViewModel();
            AboutView=new AboutViewModel();
            CurrentDownloads=new ObservableCollection<NovelDownloader>();
            Names = new ObservableCollection<string>();
            CurrentView = DownloadsView;

            OpenDownloadsView = new RelayCommand((o) => 
            {
                CurrentView = DownloadsView;
            });

            OpenSettingsView = new RelayCommand((o) =>
            {
                CurrentView = SettingsView;
            });

            OpenAboutView = new RelayCommand((o) =>
            {
                CurrentView = AboutView;
            });

            AddCommand = new RelayCommand(async (o) => 
            {
                NovelDownloader novel = new FreeWebNovelDownloader(UrlTextBox);
                await novel.GetNovelInfo();
                CurrentDownloads.Add(novel);
                UrlTextBox = "";
            });

            DownloadCommand = new RelayCommand(async (o) => 
            {
                if(SelectedDownload != null)
                    await SelectedDownload.DownloadNovel();
            });
        }

    }
}
