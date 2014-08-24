using Grundwortschatz.Models;
using Grundwortschatz.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;

namespace Grundwortschatz.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        protected ResourceManager _resourceManager;
        private DelegateCommand _goToMenu;
        internal DelegateCommand _goToNextLogicalPageCommand;

        public ViewModel()
        {
            this.PageTitle = GetResString("PageTitle");//_resourceManager.GetString("PageTitle");//"English-Russian essential words";
            this.MenuCommandText = GetResString("MainPagePageHeader");
            _goToMenu = new DelegateCommand(this.GoToMenu);
        }

        private string _menuCommandText;
        public System.Windows.Input.ICommand GoToMenuCommand { get { return _goToMenu; } }
        public string MenuCommandText { get { return _menuCommandText; } set { _menuCommandText = value; } }

        public string GetResString(string key)
        {
            return Helpers.GetResString(key);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _pageHeader;
        public string PageHeader { get { return _pageHeader.ToLower(); } set { _pageHeader = value; } }

        private string _pageTitle;
        public string PageTitle { get { return _pageTitle.ToUpper(); } set { _pageTitle = value; } }

        public string NextLogicalPageName(string currentPage)
        {
            switch (currentPage.ToLower())
            {
                case "topics":
                case "vocabulary":
                    return GetResString("MainPageButtonContentFlashCards").ToLower() + " >>";
                case "flash":
                    return GetResString("MainPageButtonContentPractice").ToLower() + " >>";
                case "practice":
                    return GetResString("MainPageButtonContentPracticeExam").ToLower() + " >>";
                case "practiceexam":
                    return GetResString("MainPageButtonContentExam").ToLower() + " >>";
                case "exam":
                    return GetResString("StudyItemsForThisTagButtonText").ToLower();
            }

            return "";
        }

        public string NextPageName { get { return ">>"; } }
        public System.Windows.Input.ICommand GoToNextLogicalPageCommand { get { return _goToNextLogicalPageCommand; } }

        internal void GoToMenu(object obj)
        {
            //(App.Current as App).RootFrame.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
            App.RootFrame.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
        }

        internal void GoToFlashCards(object obj)
        {
            //(App.Current as App).RootFrame.Navigate(new Uri("/Views/flash.xaml", UriKind.Relative));
            App.RootFrame.Navigate(new Uri("/Views/flash.xaml", UriKind.Relative));
        }

        internal void GoToPractice(object obj)
        {
            //(App.Current as App).RootFrame.Navigate(new Uri("/Views/practice2.xaml", UriKind.Relative));
            App.RootFrame.Navigate(new Uri("/Views/practice.xaml", UriKind.Relative));
        }

        internal void GoToSelectTopics(object obj)
        {
            //(App.Current as App).RootFrame.Navigate(new Uri("/Views/selectTopics.xaml", UriKind.Relative));
            App.RootFrame.Navigate(new Uri("/Views/selectTopics.xaml", UriKind.Relative));
        }

        internal void GoToStudyVocabulary(object obj)
        {
            //(App.Current as App).RootFrame.Navigate(new Uri("/Views/vocabulary.xaml", UriKind.Relative));
            App.RootFrame.Navigate(new Uri("/Views/vocabulary.xaml", UriKind.Relative));
        }

        internal void GoToTagsVocabulary(object obj)
        {
            string tagName = obj.ToString();
            //and navigate to the vocabulary page with tagName argument
            //(App.Current as App).RootFrame.Navigate(new Uri("/Views/vocabulary.xaml?t=" + tagName, UriKind.Relative));
            App.RootFrame.Navigate(new Uri("/Views/vocabulary.xaml?t=" + tagName, UriKind.Relative));
        }

        internal void GoToExamTest(object obj)
        {
            //(App.Current as App).RootFrame.Navigate(new Uri("/Views/exam.xaml", UriKind.Relative));
            App.RootFrame.Navigate(new Uri("/Views/exam.xaml", UriKind.Relative));
        }

        internal void GoToPracticeTest(object obj)//if parameter p is present, it indicates practice
        {
            //(App.Current as App).RootFrame.Navigate(new Uri("/Views/exam.xaml?p=0", UriKind.Relative));
            App.RootFrame.Navigate(new Uri("/Views/exam.xaml?p=0", UriKind.Relative));
        }

        internal void GoToAboutPage(object obj)
        {
            //(App.Current as App).RootFrame.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
            App.RootFrame.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }
    }
}
