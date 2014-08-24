using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace RenamerUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string OLD_NEW_NAME_SEPARATOR = "//";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void folderSelector_Click(object sender, RoutedEventArgs e)
        {
            var d = new FolderBrowserDialog();
            DialogResult dr = d.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                folderSelection.Text = d.SelectedPath;
            }
            results.Content = string.Empty;
        }

        private void doItButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            DirectoryInfo di = new DirectoryInfo(folderSelection.Text);
            if (di != null)
            {
                Directory.SetCurrentDirectory(folderSelection.Text);
                FileInfo[] fi = di.GetFiles();
                foreach (FileInfo f in fi)
                {
                    string oldName = f.Name;
                    string newName = f.Name.Replace(replaceWhat.Text, replaceWith.Text);
                    if (oldName.CompareTo(newName) != 0)
                    {
                        File.Move(oldName, newName);
                        i++;
                        sb.Append(Environment.NewLine);
                        sb.Append(oldName + " -> " + newName);
                    }
                }

                if (includeDirectories.IsChecked == true)
                {
                    DirectoryInfo[] dii = di.GetDirectories();
                    foreach (DirectoryInfo dinfo in dii)
                    {
                        string oldName = dinfo.Name;
                        string newName = dinfo.Name.Replace(replaceWhat.Text, replaceWith.Text);
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

            results.Content = sb.ToString();
        }

        private void PreviewChanges()
        {
            List<string> filesToBeChanged = new List<string>();
            List<string> foldersToBeChanged = new List<string>();

            StringBuilder sb = new StringBuilder();
            DirectoryInfo di = new DirectoryInfo(folderSelection.Text);
            if (di != null)
            {
                Directory.SetCurrentDirectory(folderSelection.Text);
                FileInfo[] fi = di.GetFiles();
                foreach (FileInfo f in fi)
                {
                    string oldName = f.Name;
                    string newName = f.Name.Replace(replaceWhat.Text, replaceWith.Text);
                    if (oldName.CompareTo(newName) != 0)
                    {
                        filesToBeChanged.Add(oldName + OLD_NEW_NAME_SEPARATOR + newName);
                    }
                }

                if (includeDirectories.IsChecked == true)
                {
                    DirectoryInfo[] dii = di.GetDirectories();
                    foreach (DirectoryInfo dinfo in dii)
                    {
                        string oldName = dinfo.Name;
                        string newName = dinfo.Name.Replace(replaceWhat.Text, replaceWith.Text);
                        if (oldName.CompareTo(newName) != 0)
                        {
                            foldersToBeChanged.Add(oldName + OLD_NEW_NAME_SEPARATOR + newName);
                        }
                    }
                }

                foreach(string s in filesToBeChanged)
                {
                    string[] names = s.Split(new string[] {OLD_NEW_NAME_SEPARATOR}, StringSplitOptions.RemoveEmptyEntries);
                    sb.Append("File: " + names[0] + " -> " + names[1] + Environment.NewLine);
                }

                foreach(string s in foldersToBeChanged)
                {
                    string[] names = s.Split(new string[] { OLD_NEW_NAME_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
                    sb.Append("Dir:" + names[0] + " -> " + names[1] + Environment.NewLine);
                }

                sb.Append(Environment.NewLine);
                sb.Append(filesToBeChanged.Count+foldersToBeChanged.Count + " replacements can be made");

            }
            else sb.Append("path not found");

            results.Content = sb.ToString();

        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            replaceWhat.Text = string.Empty;
            replaceWith.Text = string.Empty;
            folderSelection.Text = string.Empty;
            includeDirectories.IsChecked = false;
        }

        private void preview_Click(object sender, RoutedEventArgs e)
        {
            PreviewChanges();
        }
    }
}
