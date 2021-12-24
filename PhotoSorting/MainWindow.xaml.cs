using Ookii.Dialogs.Wpf;
using PhotoSorting.ViewModels;
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

namespace PhotoSorting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowVM _context = new MainWindowVM();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _context;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog ofd = new VistaFolderBrowserDialog();
            if (ofd.ShowDialog() == true)
            {
                _context.DirectoryPath = ofd.SelectedPath;
            }
        }

        private async void StartAnalise(object sender, RoutedEventArgs e)
        {
            await _context.Analise();
        }
    }
}
