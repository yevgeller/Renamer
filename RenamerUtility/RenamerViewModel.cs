using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RenamerUtility
{
    public class RenamerViewModel : ViewModel
    {
        string OLD_NEW_NAME_SEPARATOR = "//";

        public RenamerViewModel()
        {
            InitCommands();
        }

        private void InitCommands()
        {
            _previewChangesCommand = new DelegateCommand(this.PreviewChangesCommandAction, this.CanPreviewChanges);
            _clearValuesCommand = new DelegateCommand(this.ClearValuesCommandAction, this.CanClearValues);
            _renameFilesCommand = new DelegateCommand(this.RenameFilesCommandAction, this.CanRenameFiles);
        }

        private string _previewResults;
        public string PreviewResults
        {
            get { return _previewResults; }
            set
            {
                if (_previewResults == value) return;
                _previewResults = value;
                OnPropertyChanged("PreviewResults");
            }
        }


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

        private bool _includeDirectiories;
        public bool IncludeDirectories
        {
            get { return _includeDirectiories; }
            set
            {
                if (_includeDirectiories == value) return;
                _includeDirectiories = value;
                OnPropertyChanged("IncludeDirectories");
            }
        }

        DelegateCommand _previewChangesCommand;
        public ICommand PreviewChangesCommand { get { return _previewChangesCommand; } }
        private void PreviewChangesCommandAction(object obj)
        {
            List<string> filesToBeChanged = new List<string>();
            List<string> foldersToBeChanged = new List<string>();

            StringBuilder sb = new StringBuilder();
            DirectoryInfo di = new DirectoryInfo(this.FolderSelection);
            if (di != null)
            {
                Directory.SetCurrentDirectory(this.FolderSelection);
                FileInfo[] fi = di.GetFiles();
                foreach (FileInfo f in fi)
                {
                    string oldName = f.Name;
                    string newName = f.Name.Replace(this.ReplaceWhat, this.ReplaceWith);
                    if (oldName.CompareTo(newName) != 0)
                    {
                        filesToBeChanged.Add(oldName + OLD_NEW_NAME_SEPARATOR + newName);
                    }
                }

                if (this.IncludeDirectories == true)
                {
                    DirectoryInfo[] dii = di.GetDirectories();
                    foreach (DirectoryInfo dinfo in dii)
                    {
                        string oldName = dinfo.Name;
                        string newName = dinfo.Name.Replace(this.ReplaceWhat, this.ReplaceWith);
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

            this.PreviewResults = sb.ToString();
        }
        //add the following line to ViewModel constructor or wherever else you initialize your commands:
        private bool CanPreviewChanges(object obj)
        {
            bool result = this.ReplaceWhat.Trim().Length > 0 &&
                this.ReplaceWith.Trim().Length > 0 &&
                this.FolderSelection.Trim().Length > 0;
            return result;
        }
        
        DelegateCommand _clearValuesCommand;
        public ICommand ClearValuesCommand { get { return _clearValuesCommand; } }
        private void ClearValuesCommandAction(object obj)
        {
            this.ReplaceWhat = string.Empty;
            this.ReplaceWith = string.Empty;
            this.FolderSelection = string.Empty;
            this.IncludeDirectories = false;
        }
        //add the following line to ViewModel constructor or wherever else you initialize your commands:
        private bool CanClearValues(object obj)
        {
            return true;
        }

        
        DelegateCommand _renameFilesCommand;
        public ICommand RenameFilesCommand { get { return _renameFilesCommand; } }
        private void RenameFilesCommandAction(object obj)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            DirectoryInfo di = new DirectoryInfo(this.FolderSelection);
            if (di != null)
            {
                Directory.SetCurrentDirectory(this.FolderSelection);
                FileInfo[] fi = di.GetFiles();
                foreach (FileInfo f in fi)
                {
                    string oldName = f.Name;
                    string newName = f.Name.Replace(this.ReplaceWhat, this.ReplaceWith);
                    if (oldName.CompareTo(newName) != 0)
                    {
                        File.Move(oldName, newName);
                        i++;
                        sb.Append(Environment.NewLine);
                        sb.Append(oldName + " -> " + newName);
                    }
                }

                if (this.IncludeDirectories == true)
                {
                    DirectoryInfo[] dii = di.GetDirectories();
                    foreach (DirectoryInfo dinfo in dii)
                    {
                        string oldName = dinfo.Name;
                        string newName = dinfo.Name.Replace(this.ReplaceWhat, this.ReplaceWith);
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

            this.PreviewResults = sb.ToString();
        }

        private bool CanRenameFiles(object obj)
        {
            return this.CanPreviewChanges(null);
        }
      
		
      

    }
}
