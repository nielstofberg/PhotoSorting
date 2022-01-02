using Microsoft.EntityFrameworkCore;
using PhotoSorting.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private List<string> _exts = new List<string>();
        private string _dirPath = @"c:\";
        private string _progress = "";
        private readonly DataContext _context;
        private bool _isCcanceled = false;
        private int _photosChecked = 0;
        private int _photosAdded = 0;
        private Photo _selectedPhoto;
        private int _videosChecked = 0;
        private int _videosAdded = 0;
        private VideoFile _selectedVideo;

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
                IList<Photo> ret = _context.Photos
                    .OrderBy(p=> p.DoubleSetId)
                    .ThenBy(p=>p.Path).ToList();
                return ret; }
        }

        public IList<VideoFile> VideoFiles
        {
            get
            {
                IList<VideoFile> ret = _context.VideoFiles
                    .OrderBy(p => p.DoubleSetId)
                    .ThenBy(p => p.Path).ToList();
                return ret;
            }
        }

        public string Summary
        {
            get 
            {
                int t = _context.Photos.Count(); //Total number of photos
                int d = _context.DoubleSets.Count(); // Double sets
                int di = _context.Photos.Where(p => p.DoubleSetId > 0).Count(); //Photos included in a double set
                string ret = $"Total Pictures:\t{t}\r\n";
                ret += $"Double Sets:\t{d}\r\n";
                ret += $"Individual Photos:\t{t - di + d}";
                return ret; 
            }
        }

        public string VideoSummary
        {
            get
            {
                int t = _context.VideoFiles.Count(); //Total number of photos
                int d = _context.VideoFiles.Distinct().Count(); // Double sets
                int di = _context.VideoFiles.Where(p => p.DoubleSetId > 0).Count(); //Photos included in a double set
                string ret = $"Total Pictures:\t{t}\r\n";
                ret += $"Double Sets:\t{d}\r\n";
                ret += $"Individual Photos:\t{t - di + d}";
                return ret;
            }
        }

        public string ImageSrc { get { return _selectedPhoto?.FullName; } }

        public string VideoSrc { get { return _selectedVideo?.FullName; } }

        public MainWindowVM(DataContext context)
        {
            _context = context;
        }

        public void SetImage(Photo ph)
        {
            _selectedPhoto = ph;
            NotifyPropertyChanged(nameof(ImageSrc));
        }

        internal void SetVideo(VideoFile vid)
        {
            _selectedVideo = vid;
            NotifyPropertyChanged(nameof(VideoSrc));
        }

        internal async Task DeleteDoubles()
        {
            Progress = "Deleting photos";
            await DeletePhotos();
            Progress = "Deleting videos";
            await DeleteVideos();

            var ds = await _context.DoubleSets.Where(d => d.Photos.Count < 2 || d.VideoFiles.Count < 2).ToListAsync();
            _context.DoubleSets.RemoveRange(ds);
            _context.SaveChanges();
            Progress = "Complete";
            NotifyPropertyChanged(nameof(Photos));
            NotifyPropertyChanged(nameof(VideoFiles));
        }

        private async Task DeletePhotos()
        {
            var photos = await _context.Photos.Where(p => p.ToDelete == true).ToListAsync();
            foreach (var photo in photos)
            {
                try
                {
                    File.Delete(photo.FullName);
                    photo.Deleted = true;
                }
                catch (Exception ex)
                {
                }
            }
            _context.Photos.UpdateRange(photos);
            _context.SaveChanges();

            photos = await _context.Photos.Where(p => p.Deleted == true).ToListAsync();
            _context.Photos.RemoveRange(photos);
        }
        
        private async Task DeleteVideos()
        {
            var videos = await _context.VideoFiles.Where(p => p.ToDelete == true).ToListAsync();
            foreach (var video in videos)
            {
                try
                {
                    File.Delete(video.FullName);
                    video.Deleted = true;
                }
                catch (Exception ex)
                {
                }
            }
            _context.VideoFiles.UpdateRange(videos);
            _context.SaveChanges();

            videos = await _context.VideoFiles.Where(p => p.Deleted == true).ToListAsync();
            _context.VideoFiles.RemoveRange(videos);
        }

        internal async Task MarkDelete()
        {

            var doublesets = await _context.DoubleSets.Include(d => d.Photos).Include(d => d.VideoFiles).ToListAsync();
            doublesets.ForEach(p => p.Photos = p.Photos.OrderBy(h => h.Path).ToList());
            doublesets.ForEach(p => p.VideoFiles = p.VideoFiles.OrderBy(h => h.Path).ToList());
            foreach (var ds in doublesets)
            {
                for (int i = 0; i < ds.Photos.Count-1; i++)
                {
                    ds.Photos[i].ToDelete = true;
                }
                for (int i = 0; i < ds.VideoFiles.Count - 1; i++)
                {
                    ds.VideoFiles[i].ToDelete = true;
                }
            }
            _context.DoubleSets.UpdateRange(doublesets);
            _context.SaveChanges();
            NotifyPropertyChanged(nameof(Photos));
            NotifyPropertyChanged(nameof(VideoFiles));
        }

        public async Task IdentifyDoubles()
        {
            await ClearDoubles();
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('DoubleSets', RESEED, 0)");
            await FindPhotoDoubles();
            await FindVideoDoubles();
            _context.SaveChanges();
            Progress = $"Complete";
            NotifyPropertyChanged(nameof(Photos));
            NotifyPropertyChanged(nameof(VideoFiles));
        }

        private async Task FindPhotoDoubles()
        {
            var photos = (await _context.Photos.ToListAsync()).OrderBy(p => p.Size).ThenBy(p => p.Path).ToArray();
            DoubleSet ds = null;
            _isCcanceled = false;
            int setCount = 0;

            for (int i = 0; i < photos.Length; i++)
            {
                try
                {
                    if (_isCcanceled) break;
                    if (ds != null)
                    {
                        if (ds.Photos.Count > 0 &&
                          photos[i].Name == ds.Photos[0].Name &&
                          photos[i].Size == ds.Photos[0].Size)
                        {
                            //photos[i].DoubleSetId = ds.Id;
                            ds.Photos.Add(photos[i]);
                            //_context.Photos.Update(photos[i]);
                        }
                        else
                        {
                            _context.DoubleSets.Add(ds);
                            if (setCount % 1000 == 0)
                            {
                                Progress = $"Doubles counted: {setCount} ";
                                await _context.SaveChangesAsync();
                            }
                            ds = null;
                        }
                    }
                    if (ds == null)
                    {
                        if (photos[i].DoubleSetId != null)
                        {
                            ds = _context.DoubleSets.First(d => d.Id == photos[i].DoubleSetId);
                            ds.Photos = new List<Photo> { photos[1] };
                        }
                        else if (i < photos.Length - 1 &&
                           photos[i].Name == photos[i + 1].Name &&
                           photos[i].Size == photos[i + 1].Size)
                        {
                            ds = new DoubleSet();
                            setCount++;
                            ds.Photos.Add(photos[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private async Task FindVideoDoubles()
        {
            var videos = (await _context.VideoFiles.ToListAsync()).OrderBy(p => p.Size).ThenBy(p => p.Path).ToArray();
            DoubleSet ds = null;
            _isCcanceled = false;
            int setCount = 0;

            for (int i = 0; i < videos.Length; i++)
            {
                try
                {
                    if (_isCcanceled) break;
                    if (ds != null)
                    {
                        if (ds.VideoFiles.Count > 0 &&
                          videos[i].Name == ds.VideoFiles[0].Name &&
                          videos[i].Size == ds.VideoFiles[0].Size)
                        {
                            //photos[i].DoubleSetId = ds.Id;
                            ds.VideoFiles.Add(videos[i]);
                            //_context.Photos.Update(photos[i]);
                        }
                        else
                        {
                            _context.DoubleSets.Add(ds);
                            if (setCount % 1000 == 0)
                            {
                                Progress = $"Doubles counted: {setCount} ";
                                await _context.SaveChangesAsync();
                            }
                            ds = null;
                        }
                    }
                    if (ds == null)
                    {
                        if (videos[i].DoubleSetId != null)
                        {
                            ds = _context.DoubleSets.First(d => d.Id == videos[i].DoubleSetId);
                            ds.VideoFiles = new List<VideoFile> { videos[1] };
                        }
                        else if (i < videos.Length - 1 &&
                           videos[i].Name == videos[i + 1].Name &&
                           videos[i].Size == videos[i + 1].Size)
                        {
                            ds = new DoubleSet();
                            setCount++;
                            ds.VideoFiles.Add(videos[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private async Task ClearDoubles()
        {
            var photos = await _context.Photos.Where(p=> p.DoubleSetId>0).ToArrayAsync();
            var doubles = await _context.DoubleSets.ToArrayAsync();

            foreach (var ph in photos)
            {
                ph.DoubleSetId = null;
                ph.ToDelete = false;
            }

            _context.Photos.UpdateRange(photos);
            _context.DoubleSets.RemoveRange(doubles);
            _context.SaveChanges();
        }

        public async Task CollectImages()
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
                NotifyPropertyChanged(nameof(Photos));
                NotifyPropertyChanged(nameof(VideoFiles));
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
            NotifyPropertyChanged(nameof(Photos));
            NotifyPropertyChanged(nameof(VideoFiles));
        }

        private async Task LogSubdirectories(DirectoryInfo di)
        {
            Progress = $"Checked: {_photosChecked + _videosChecked} - Photos Added: {_photosAdded} - Videos Added: {_videosAdded} - Current directory: {di.FullName}";
            await logFiles(di.GetFiles());
            await _context.SaveChangesAsync();
            foreach (var dir in di.GetDirectories())
            {
                if (_isCcanceled) break;
                await LogSubdirectories(dir);
            }
        }

        private async Task logFiles(FileInfo[] files)
        {
            foreach (var file in files)
            {
                if (_isCcanceled) break;
                if (Photo.PHOTO_EXT.Contains(file.Extension.ToLower()))
                {
                    await addPhoto(file);
                }
                else if (VideoFile.VIDEO_EXT.Contains(file.Extension.ToLower()))
                {
                    await addVideo(file);
                }
            }
        }

        private async Task addPhoto(FileInfo file)
        {
            Photo existing = null;
            Photo ph = null;
            try
            {
                ph = await Photo.GetPhoto(file);
                _photosChecked++;
                existing = _context.Photos.First(p => p.FullName == ph.FullName);
            }
            catch (Exception ex)
            {
                var ext = file.Extension;
                if (!_exts.Contains(ext))
                    _exts.Add(ext);
            }
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

        private async Task addVideo(FileInfo file)
        {
            VideoFile existing = null;
            VideoFile vid = null;
            try
            {
                vid = await VideoFile.GetVideo(file);
                _videosChecked++;
                existing = _context.VideoFiles.First(p => p.FullName == vid.FullName);
            }
            catch (Exception ex)
            {
                var ext = file.Extension;
                if (!_exts.Contains(ext))
                    _exts.Add(ext);
            }
            if (existing != null && vid != null)
            {
                existing.Size = vid.Size;
                existing.DateCreated = vid.DateCreated;
                existing.DateModified = vid.DateModified;
                _context.VideoFiles.Update(existing);
            }
            else if (existing == null && vid != null)
            {
                _context.VideoFiles.Add(vid);
                _videosAdded++;
            }
        }

        public void CancelProcess()
        {
            _isCcanceled = true;
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
