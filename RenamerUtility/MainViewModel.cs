using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RenamerUtility
{
    public class MainViewModel : ViewModel
    {
        string OLD_NEW_NAME_SEPARATOR = "//";

        public MainViewModel ()
	{
            InitCommands();
            InitProperties();
	}

        #region Properties
        private void InitProperties()
        {
            this.FolderSelection = @"E:\_test";
            this.ReplaceWhat = "ao";
            this.ReplaceWith = "oa";
            this.RegexPattern = @"[\d]";
            this.IncludeFiles = true;
            this.IncludeDirectories = false;
            this.UseRegularExpressions = false;
        }
        
        //the textbox that contains the folder that the program is working with
        private string _folderSelection;
        public string FolderSelection
        {
            get { return _folderSelection; }
            set
            {
                if (_folderSelection == value) return;
                _folderSelection = value;
                OnPropertyChanged("FolderSelection");
            }
        }
        //replacing what
        private string _replaceWhat;
        public string ReplaceWhat
        {
            get { return _replaceWhat; }
            set
            {
                if (_replaceWhat == value) return;
                _replaceWhat = value;
                OnPropertyChanged("ReplaceWhat");
            }
        }
        //replacing with
        private string _replaceWith;
        public string ReplaceWith
        {
            get { return _replaceWith; }
            set
            {
                if (_replaceWith == value) return;
                _replaceWith = value;
                OnPropertyChanged("ReplaceWith");
            }
        }

        private string _regexPattern;
        public string RegexPattern
        {
            get { return _regexPattern; }
            set
            {
                if (_regexPattern == value) return;
                _regexPattern = value;
                OnPropertyChanged("RegexPattern");
            }
        }

        private bool _includeFiles;
        public bool IncludeFiles
        {
            get { return _includeFiles; }
            set
            {
                if (_includeFiles == value) return;
                _includeFiles = value;
                OnPropertyChanged("IncludeFiles");
            }
        }

        private bool _includeDirectories;
        public bool IncludeDirectories
        {
            get { return _includeDirectories; }
            set
            {
                if (_includeDirectories == value) return;
                _includeDirectories = value;
                OnPropertyChanged("IncludeDirectories");
            }
        }

        private bool _useRegularExpressions;
        public bool UseRegularExpressions
        {
            get { return _useRegularExpressions; }
            set
            {
                if (_useRegularExpressions == value) return;
                _useRegularExpressions = value;
                OnPropertyChanged("UseRegularExpressions");
            }
        }

        private string _results;
        public string Results
        {
            get { return _results; }
            set
            {
                if (_results == value) return;
                _results = value;
                OnPropertyChanged("Results");
            }
        }		
        #endregion

        #region commands
        private void InitCommands()
        {
            _renameCommand = new DelegateCommand(this.RenameCommandAction, this.CanRename);
            _previewChangesCommand = new DelegateCommand(this.PreviewChangesCommandAction, this.CanPreviewChanges);
            _clearInputValuesCommand = new DelegateCommand(this.ClearInputValuesCommandAction, this.CanClearInputValues);
        }

        private void CanExecute_Changed()
        {
            _renameCommand.RaiseCanExecuteChanged();
            _previewChangesCommand.RaiseCanExecuteChanged();
        }

        DelegateCommand _renameCommand;
        public ICommand RenameCommand { get { return _renameCommand; } }
        private void RenameCommandAction(object obj)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            DirectoryInfo di = new DirectoryInfo(FolderSelection);
            if (di != null)
            {
                Directory.SetCurrentDirectory(FolderSelection);
                FileInfo[] fi = di.GetFiles();
                foreach (FileInfo f in fi)
                {
                    string oldName = f.Name;
                    string newName = f.Name;
                    if (UseRegularExpressions)
                    {
                        if (Regex.IsMatch(oldName, RegexPattern))
                        {
                            Regex rgx = new Regex(RegexPattern);
                            newName = rgx.Replace(oldName, ReplaceWith);
                        }
                    }
                    else
                        newName = f.Name.Replace(ReplaceWhat, ReplaceWith);

                    if (oldName.CompareTo(newName) != 0)
                    {
                        File.Move(oldName, newName);
                        i++;
                        sb.Append(Environment.NewLine);
                        sb.Append(oldName + " -> " + newName);
                    }
                }

                if (IncludeDirectories)
                {
                    DirectoryInfo[] dii = di.GetDirectories();
                    foreach (DirectoryInfo dinfo in dii)
                    {
                        string oldName = dinfo.Name;
                        string newName = dinfo.Name;
                        if (UseRegularExpressions)
                        {
                            if (Regex.IsMatch(oldName, RegexPattern))
                            {
                                Regex rgx = new Regex(RegexPattern);
                                newName = rgx.Replace(oldName, ReplaceWith);
                            }
                        }
                        else
                            newName = dinfo.Name.Replace(ReplaceWhat, ReplaceWith);
                        if (oldName.CompareTo(newName) != 0)
                        {
                            Directory.Move(oldName, newName);
                            i++;
                            sb.Append(Environment.NewLine);
                            sb.Append("Folder: " + oldName + " -> " + newName);
                        }
                    }
                }

                sb.Append(Environment.NewLine);
                sb.Append(i.ToString() + " replacements made");

            }
            else sb.Append("path not found");

            Results = sb.ToString();
        }
        private bool CanRename(object obj)
        {
            return true;
        }

        DelegateCommand _previewChangesCommand;
        public ICommand PreviewChangesCommand { get { return _previewChangesCommand; } }
        private void PreviewChangesCommandAction(object obj)
        {
            List<string> filesToBeChanged = new List<string>();
            List<string> foldersToBeChanged = new List<string>();

            StringBuilder sb = new StringBuilder();
            DirectoryInfo di = new DirectoryInfo(FolderSelection);
            if (di != null)
            {
                Directory.SetCurrentDirectory(FolderSelection);
                FileInfo[] fi = di.GetFiles();
                foreach (FileInfo f in fi)
                {
                    string oldName = f.Name;
                    string newName = f.Name;
                    if (UseRegularExpressions)
                    {
                        if (Regex.IsMatch(oldName, RegexPattern))
                        {
                            Regex rgx = new Regex(RegexPattern);
                            newName = rgx.Replace(oldName, ReplaceWith);
                        }
                    }
                    else
                        newName = f.Name.Replace(ReplaceWhat, ReplaceWith);

                    if (oldName.CompareTo(newName) != 0)
                    {
                        filesToBeChanged.Add(oldName + OLD_NEW_NAME_SEPARATOR + newName);
                    }
                }

                if (IncludeDirectories)
                {
                    DirectoryInfo[] dii = di.GetDirectories();
                    foreach (DirectoryInfo dinfo in dii)
                    {
                        string oldName = dinfo.Name;
                        string newName = dinfo.Name;
                        if (UseRegularExpressions)
                        {
                            if (Regex.IsMatch(oldName, RegexPattern))
                            {
                                Regex rgx = new Regex(RegexPattern);
                                newName = rgx.Replace(oldName, ReplaceWith);
                            }
                        }
                        else
                            newName = dinfo.Name.Replace(ReplaceWhat, ReplaceWith);

                        if (oldName.CompareTo(newName) != 0)
                        {
                            foldersToBeChanged.Add(oldName + OLD_NEW_NAME_SEPARATOR + newName);
                        }
                    }
                }

                foreach (string s in filesToBeChanged)
                {
                    string[] names = s.Split(new string[] { OLD_NEW_NAME_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
                    sb.Append("File: " + names[0] + " -> " + names[1] + Environment.NewLine);
                }

                foreach (string s in foldersToBeChanged)
                {
                    string[] names = s.Split(new string[] { OLD_NEW_NAME_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
                    sb.Append("Dir:" + names[0] + " -> " + names[1] + Environment.NewLine);
                }

                sb.Append(Environment.NewLine);
                sb.Append(filesToBeChanged.Count + foldersToBeChanged.Count + " replacements can be made");

            }
            else sb.Append("path not found");

            Results = sb.ToString();
        }
        private bool CanPreviewChanges(object obj)
        {
            return true;
        }

        DelegateCommand _clearInputValuesCommand;
        public ICommand ClearInputValuesCommand { get { return _clearInputValuesCommand; } }
        private void ClearInputValuesCommandAction(object obj)
        {
            ReplaceWhat = string.Empty;
            ReplaceWith = string.Empty;
            FolderSelection = string.Empty;
            IncludeDirectories = false;
            UseRegularExpressions = false;
        }
        private bool CanClearInputValues(object obj)
        {
            return true;
        }
      
      
        #endregion
    }
}
