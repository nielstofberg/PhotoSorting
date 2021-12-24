using PhotoSorting.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSorting.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private string _dirPath = @"c:\";
        private IList<Photo> _photos = new List<Photo>();
        private readonly DataContext _context;

        public string DirectoryPath
        {
            get { return _dirPath; }
            set
            {
                _dirPath = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindowVM(DataContext context)
        {
            _context = context;
        }

        public async Task Analise()
        {
            DirectoryInfo di = new DirectoryInfo(DirectoryPath);
            var files = di.GetFiles();
            foreach (var file in files)
            {
                try
                {
                    Photo ph = await Photo.GetPhoto(file);
                    _photos.Add(ph);
                }
                catch { }
            }
        }


        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
