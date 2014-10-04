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
        //string OLD_NEW_NAME_SEPARATOR = "//";

        public MainViewModel()
        {
            InitCommands();
            InitProperties();
        }

        private void PreviewAndOrRename(bool rename)
        {
            DirectoryInfo di = new DirectoryInfo(FolderSelection);
            if (di != null)
            {
                Directory.SetCurrentDirectory(FolderSelection);
                FileInfo[] fi = di.GetFiles();
                DirectoryInfo[] dii = di.GetDirectories();
                this.GetItemsToBeRenamed(fi, dii);
                if (rename)
                {
                    RenameItems();
                }
                ProvideResultsFeedback();
            }
            else
                Results = "Cannot find path.";
        }

        public void GetItemsToBeRenamed(FileInfo[] fi, DirectoryInfo[] di)
        {
            List<ItemForRenaming> ret = new List<ItemForRenaming>();
            if (FolderSelection != null)
            {
                Directory.SetCurrentDirectory(FolderSelection); //?

                if (fi.Length > 0)
                {
                    foreach (FileInfo f in fi)
                    {
                        ItemForRenaming ifr = new ItemForRenaming { NewName = f.Name, OldName = f.Name, IsFile = true };
                        if (UseRegularExpressions)
                        {
                            if (Regex.IsMatch(ifr.OldName, RegexPattern))
                            {
                                Regex rgx = new Regex(RegexPattern);
                                ifr.NewName = rgx.Replace(ifr.OldName, ReplaceWith);
                            }
                        }
                        else
                            ifr.NewName = f.Name.Replace(ReplaceWhat, ReplaceWith);

                        ret.Add(ifr);
                    }
                }
                if (di.Length > 0 && IncludeDirectories)
                {
                    foreach (DirectoryInfo dinfo in di)
                    {
                        ItemForRenaming ifr = new ItemForRenaming { NewName = dinfo.Name, OldName = dinfo.Name, IsFile = false };
                        if (UseRegularExpressions)
                        {
                            if (Regex.IsMatch(ifr.OldName, RegexPattern))
                            {
                                Regex rgx = new Regex(RegexPattern);
                                ifr.NewName = rgx.Replace(ifr.OldName, ReplaceWith);
                            }
                        }
                        else
                            ifr.NewName = dinfo.Name.Replace(ReplaceWhat, ReplaceWith);

                        ret.Add(ifr);
                    }
                }
            }

            CheckForNamesResultingInDuplicatesAndSetProperties(ret);
            
        }

        private void CheckForNamesResultingInDuplicatesAndSetProperties(List<ItemForRenaming> src)
        {
            //TODO: check for duplicates here with ENTIRE folder
            List<ItemForRenaming> duplicates = src.GroupBy(x => x.NewName)
                .Where(a => a.Skip(1).Any())
                .SelectMany(x => x)
                .ToList<ItemForRenaming>();

            if(duplicates.Any())
            {
                foreach(ItemForRenaming i in duplicates)
                {
                    src.RemoveAll(x => x.Equals(i));
                }
            }

            ItemsThatCanBeRenamed = src;
            ItemsThatCANNOTBeRenamed = duplicates;
        }

        private void RenameItems()
        {
            foreach (ItemForRenaming i in ItemsThatCanBeRenamed)
            {
                if (i.NeedsRenaming())
                {
                    if (i.IsFile) File.Move(i.OldName, i.NewName);
                    else Directory.Move(i.OldName, i.NewName);
                }
            }
        }

        private void ProvideResultsFeedback()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ItemForRenaming i in ItemsThatCanBeRenamed)
            {
                sb.Append(i.ToString());
                sb.Append(Environment.NewLine);
            }
            if (this.ItemsThatCANNOTBeRenamed.Any())
            {
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append("--- The following items could not be renamed as the renaming would result in duplicate names:");
                sb.Append(Environment.NewLine);
                foreach (ItemForRenaming i in ItemsThatCANNOTBeRenamed)
                {
                    sb.Append(i.ToString());
                    sb.Append(Environment.NewLine);
                }
            }
            this.Results = sb.ToString();
        }

        #region Properties
        private void InitProperties()
        {
            this.FolderSelection = @"C:\Users\YEVGENIY\Documents\_test";
            this.ReplaceWhat = "1";
            this.ReplaceWith = "1";
            this.RegexPattern = @"\d+";
            this.IncludeFiles = true;
            this.IncludeDirectories = true;
            this.UseRegularExpressions = false;
        }

        public List<ItemForRenaming> ItemsThatCanBeRenamed { get; set; }
        public List<ItemForRenaming> ItemsThatCANNOTBeRenamed { get; set; }

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

        #region Commands
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
            PreviewAndOrRename(true);
        }
        private bool CanRename(object obj)
        {
            return CanPreviewOrRename(null);
        }

        DelegateCommand _previewChangesCommand;
        public ICommand PreviewChangesCommand { get { return _previewChangesCommand; } }
        private void PreviewChangesCommandAction(object obj)
        {
            PreviewAndOrRename(false);
        }
        private bool CanPreviewChanges(object obj)
        {
            return CanPreviewOrRename(null);
        }

        private bool CanPreviewOrRename(object obj)
        {
            if (FolderSelection.Trim().Length == 0)
                return false;
            if (ReplaceWhat.Trim().Length == 0)
                return false;
            if (ReplaceWith.Trim().Length == 0)
                return false;
            if (UseRegularExpressions && RegexPattern.Trim().Length == 0)
                return false;

            return true;
        }

        DelegateCommand _clearInputValuesCommand;
        public ICommand ClearInputValuesCommand { get { return _clearInputValuesCommand; } }
        private void ClearInputValuesCommandAction(object obj)
        {
            List<ItemForRenaming> items = new List<ItemForRenaming>{
                    new ItemForRenaming{ OldName="1", NewName="new", IsFile=true},
                    new ItemForRenaming{ OldName="2", NewName="new1", IsFile=true},
                    new ItemForRenaming{ OldName="3", NewName="new2", IsFile=true},
                    new ItemForRenaming{ OldName="4", NewName="new", IsFile=true},
                    new ItemForRenaming{ OldName="5", NewName="new", IsFile=true},
                    new ItemForRenaming{ OldName="6", NewName="new2", IsFile=true},
                    new ItemForRenaming{ OldName="7", NewName="bb", IsFile=true}
                                          };

            List<ItemForRenaming> ab = items.GroupBy(x => x.NewName).Where(a => a.Skip(1).Any()).SelectMany(x => x).ToList<ItemForRenaming>();



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
