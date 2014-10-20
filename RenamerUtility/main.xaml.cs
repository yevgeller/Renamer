using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RenamerUtility
{
    /// <summary>
    /// Interaction logic for main.xaml
    /// </summary>
    public partial class main : Window
    {
        public main()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private void folderSelector_Click(object sender, RoutedEventArgs e)
        {
            var d = new FolderBrowserDialog();
            DialogResult dr = d.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                MainViewModel m = (MainViewModel)this.DataContext;
                m.FolderSelection = d.SelectedPath;
                m.Results = string.Empty;
            }
        }

        private void results_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                double halfHeight = System.Windows.SystemParameters.PrimaryScreenHeight / 2;

                double H = this.ActualHeight;

                if (halfHeight > this.ActualHeight)
                {
                    this.Height = halfHeight;
                }
            }
            catch{}
        }

    }
}
