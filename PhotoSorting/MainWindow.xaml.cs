using Ookii.Dialogs.Wpf;
using PhotoSorting.Models;
using PhotoSorting.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PhotoSorting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowVM _context;

        public MainWindow(DataContext context)
        {
            InitializeComponent();
            _context = new MainWindowVM(context);
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

        private async void CollectImages (object sender, RoutedEventArgs e)
        {
            await _context.CollectImages();
        }

        private async void FindDoubles(object sender, RoutedEventArgs e)
        {
            await _context.IdentifyDoubles();
        }

        private async void MarkDelete(object sender, RoutedEventArgs e)
        {
            await _context.MarkDelete();
        }

        private async void DeleteDoubles(object sender, RoutedEventArgs e)
        {
            await _context.DeleteDoubles();
        }

        private void CancelProcess(object sender, RoutedEventArgs e)
        {
            _context.CancelProcess();
        }

        private void PhotoSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Count > 0)
            {
                Photo ph = (Photo)e.AddedCells[0].Item;
                _context.SetImage(ph);
            }
        }

        private void VideoSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Count > 0)
            {
                VideoFile vid = (VideoFile)e.AddedCells[0].Item;
                _context.SetVideo(vid);
            }
        }
    }
}
