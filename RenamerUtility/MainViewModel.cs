using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RenamerUtility
{
    public class MainViewModel : ViewModel
    {
        MatchEvaluator selectedMatchEvaluatorMethod;

        public MainViewModel()
        {
            InitCommands();
            InitProperties();
        }

        //main method, true for "go ahead" with renaming, false for just a preview
        private void PreviewAndOrRename(bool rename)
        {
            Properties.Settings.Default.Save();
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
            List<ItemForRenaming> list = new List<ItemForRenaming>();
            if (FolderSelection != null)
            {
                Directory.SetCurrentDirectory(FolderSelection); //?

                if (fi.Length > 0)
                {
                    foreach (FileInfo f in fi)
                    {
                        list.Add(new ItemForRenaming { NewName = f.Name, OldName = f.Name, IsFile = true });
                    }
                }
                if (di.Length > 0 && IncludeDirectories)
                {
                    foreach (DirectoryInfo dinfo in di)
                    {
                        list.Add(new ItemForRenaming { NewName = dinfo.Name, OldName = dinfo.Name, IsFile = false });
                    }
                }
            }
            List<ItemForRenaming> ret = RenamingProcessor.Method1(list, UseRegularExpressions, RegexPattern, ReplaceWhat, ReplaceWith, selectedMatchEvaluatorMethod);
            CheckForNamesResultingInDuplicatesAndSetProperties(ret);            
        }

        #region MatchEvaluatorTransformations

        //copied from http://msdn.microsoft.com/en-us/library/cft8645c(VS.80).aspx
        private string MatchTransformation_CapText(Match m)
        {
            string x = m.ToString();
            if (char.IsLower(x[0]))
                return char.ToUpper(x[0]) + x.Substring(1, x.Length - 1);

            return x;
        }

        private string MatchTransformation_Prepend(Match m)
        {
            return ReplaceWith + m;
        }

        private string MatchTransformation_Append(Match m)
        {
            return m + ReplaceWith;
        }

        private string MatchTransformation_Remove(Match m)
        {
            return string.Empty;
        }
        #endregion

        private void CheckForNamesResultingInDuplicatesAndSetProperties(List<ItemForRenaming> src)
        {
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
            List<ItemForRenaming> willBeRenamed = ItemsThatCanBeRenamed.Where(x => x.OldName.CompareTo(x.NewName) != 0).ToList();
            List<ItemForRenaming> willNOTBeRenamed = ItemsThatCanBeRenamed.Where(x => x.OldName.CompareTo(x.NewName) == 0).ToList();

            if (willBeRenamed.Count() + willNOTBeRenamed.Count() != ItemsThatCanBeRenamed.Count())
            {
                foreach (ItemForRenaming i in ItemsThatCanBeRenamed)
                {
                    sb.Append(i.ToString());
                    sb.Append(Environment.NewLine);
                }
            }
            else
            {
                if (!willBeRenamed.Any())
                {
                    sb.Append(Environment.NewLine);
                    sb.Append("No items will be renamed under current search/replacement criteria.");
                }
                else
                {
                        sb.Append("Will be changed: ");
                        sb.Append(Environment.NewLine);
                        foreach (ItemForRenaming i in willBeRenamed)
                        {
                            sb.Append(i.ToString());
                            sb.Append(Environment.NewLine);
                        }
                    
                    if (willNOTBeRenamed.Any())
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append("Will _NOT_ be changed: ");
                        sb.Append(Environment.NewLine);
                        foreach (ItemForRenaming i in willNOTBeRenamed)
                        {
                            sb.Append(i.ToString());
                            sb.Append(Environment.NewLine);
                        }
                    }
                }
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
            this.FolderSelection = Properties.Settings.Default["FolderSelection"].ToString();
            this.ReplaceWhat = Properties.Settings.Default["ReplaceWhat"].ToString();
            this.ReplaceWith = Properties.Settings.Default["ReplaceWith"].ToString();
            this.RegexPattern = Properties.Settings.Default["RegexPattern"].ToString();
            this.IncludeFiles = Convert.ToBoolean(Properties.Settings.Default["IncludeFiles"]);
            this.IncludeDirectories = Convert.ToBoolean(Properties.Settings.Default["IncludeDirectories"]);
            this.UseRegularExpressions =Convert.ToBoolean(Properties.Settings.Default["UseRegularExpressions"]);
            this.Transformations = new List<string>();
            this.Transformations.Add("Capitalize All Words");
            this.Transformations.Add("Prepend characters");
            this.Transformations.Add("Append characters");
            this.Transformations.Add("Remove characters");
            this.SelectedTransformation = this.Transformations[1];
        }

        public List<ItemForRenaming> ItemsThatCanBeRenamed { get; set; }
        public List<ItemForRenaming> ItemsThatCANNOTBeRenamed { get; set; }
        public List<string> Transformations { get; set; }

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

        private string _selectedTransformation;
        public string SelectedTransformation
        {
            get { return _selectedTransformation; }
            set
            {
                if (_selectedTransformation == value) return;
                _selectedTransformation = value;
                OnPropertyChanged("SelectedTransformation");
                switch (_selectedTransformation.ToLower())
                {
                    case "capitalize all words":
                        selectedMatchEvaluatorMethod = this.MatchTransformation_CapText;
                        break;
                    case "prepend characters":
                        selectedMatchEvaluatorMethod = this.MatchTransformation_Prepend;
                        break;
                    case "append characters":
                        selectedMatchEvaluatorMethod = this.MatchTransformation_Append;
                        break;
                    case "remove characters":
                        selectedMatchEvaluatorMethod = this.MatchTransformation_Remove;
                        break;
                    default:
                        selectedMatchEvaluatorMethod = this.MatchTransformation_CapText;
                        break;
                }

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
