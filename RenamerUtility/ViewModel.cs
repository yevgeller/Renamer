using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;

namespace RenamerUtility
{
    public class ViewModel : INotifyPropertyChanged
    {
        protected ResourceManager _resourceManager;
        private DelegateCommand _goToMenu;
        internal DelegateCommand _goToNextLogicalPageCommand;

        public ViewModel()
        {
            //this.PageTitle = GetResString("PageTitle");//_resourceManager.GetString("PageTitle");//"English-Russian essential words";
            //this.MenuCommandText = GetResString("MainPagePageHeader");
            //_goToMenu = new DelegateCommand(this.GoToMenu);
        }

        public void SaveStringSetting(string settingName, string settingValue)
        {
            Properties.Settings.Default[settingName] = settingValue;
        }

        public string RetrieveStringSetting(string settingName)
        {
            if(Properties.Settings.Default[settingName] != null)
                return Properties.Settings.Default[settingName].ToString();
            
            return string.Empty;
        }

        //private string _menuCommandText;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //internal void GoToAboutPage(object obj)
        //{
        //    //(App.Current as App).RootFrame.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        //    App.RootFrame.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        //}
    }
}
