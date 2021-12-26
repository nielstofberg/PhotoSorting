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
        private string _progress = "";
        private readonly DataContext _context;
        private bool _isCcanceled = false;
        private int _photosChecked = 0;
        private int _photosAdded = 0;
        private Photo _selectedPhoto;

        public string DirectoryPath
        {
            get { return _dirPath; }
            set
            {
                _dirPath = value;
                NotifyPropertyChanged();
            }
        }

        public string Progress
        {
            get {  return _progress; }
            set
            {
                _progress = value;
                NotifyPropertyChanged();
            }
        }

        public IList<Photo> Photos
        {
            get {
                IList<Photo> ret = _context.Photos.ToArray();
                return ret; }
        }

        public string ImageSrc { get { return _selectedPhoto?.FullName; } }

        public MainWindowVM(DataContext context)
        {
            _context = context;
        }

        public async Task Analise()
        {
            _isCcanceled = false;
            try
            {
                DirectoryInfo di = new DirectoryInfo(DirectoryPath);
                await LogSubdirectories(di);
            }
            catch (Exception ex)
            {
                Progress = ex.Message;
                return;
            }
            if (_isCcanceled)
            {
                Progress = "Cancelled";
            }
            else
            {
                Progress = "Complete";
            }
        }

        public void CancelProcess()
        {
            _isCcanceled = true;
        }

        private async Task LogSubdirectories(DirectoryInfo di)
        {
            Progress = $"Checked: {_photosChecked} - Added: {_photosAdded} - Current directory: {di.FullName}";
            await logPhotos(di.GetFiles());
            await _context.SaveChangesAsync();
            foreach (var dir in di.GetDirectories())
            {
                if (_isCcanceled) break;
                await LogSubdirectories(dir);
            }
        }

        private async Task logPhotos(FileInfo[] files)
        {
            foreach (var file in files)
            {
                if (_isCcanceled) break;
                Photo existing = null;
                Photo ph = null;
                try
                {
                    ph = await Photo.GetPhoto(file);
                    _photosChecked++;
                    existing = _context.Photos.First(p => p.FullName == ph.FullName);
                }
                catch { }
                if (existing != null && ph != null)
                {
                    existing.DateTaken = ph.DateTaken;
                    existing.CameraMake = ph.CameraMake;
                    existing.CameraModel = ph.CameraModel;
                    existing.Size = ph.Size;
                    existing.DateCreated = ph.DateCreated;
                    existing.DateModified = ph.DateModified;
                    existing.DateAccessed = ph.DateAccessed;
                    _context.Photos.Update(existing);
                }
                else if (existing == null && ph != null)
                {
                    _context.Photos.Add(ph);
                    _photosAdded++;
                }
            }
        }

        public void SetImage(Photo ph)
        {
            _selectedPhoto = ph;
            NotifyPropertyChanged(nameof(ImageSrc));
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
