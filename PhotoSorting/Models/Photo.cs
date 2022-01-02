using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotoSorting.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime? DateTaken { get; set; } = null;
        public string CameraMake { get; set; }
        public string CameraModel { get; set; }
        public long Size { get; set; }
        public DateTime? DateCreated { get; set; } = null;
        public DateTime? DateModified { get; set; } = null;
        public DateTime? DateAccessed { get; set; } = null;
        public bool ToDelete { get; set; } = false;
        public bool Deleted { get; set; } = false;
        public int? DoubleSetId { get; set; }

        public Photo()
        {
        }

        public static readonly string[] PHOTO_EXT = new string[]
        {
            ".jpg",
            ".bmp",
            ".png"
        };

        //we init this once so that if the function is repeatedly called
        //it isn't stressing the garbage man
        private static Regex r = new Regex(":");

        //retrieves the datetime WITHOUT loading the whole image
        public static async Task<Photo> GetPhoto(FileInfo fi)
        {
            if (!PHOTO_EXT.Contains(fi.Extension.ToLower()))
            {
                throw new Exception($"{fi.Extension} is not an image file!");
            }
            Photo photo = null;
            string path = fi.FullName;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem;
                    DateTime? dt = null;
                    string make = string.Empty;
                    string model = string.Empty;
                    try
                    {
                        propItem = await Task.Run(() => myImage.GetPropertyItem(PropertyTags.DATE_TAKEN)); //36867);
                        string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                        dt = DateTime.Parse(dateTaken);
                    }
                    catch
                    { }
                    try
                    {
                        propItem = myImage.GetPropertyItem(PropertyTags.CAMERA_MAKE);
                        make = Encoding.UTF8.GetString(propItem.Value);
                    }
                    catch
                    { }
                    try
                    {
                        propItem = myImage.GetPropertyItem(PropertyTags.CAMERA_MODEL);
                        model = Encoding.UTF8.GetString(propItem.Value);
                    }
                    catch
                    { }

                    photo = new Photo
                    {
                        FullName = fi.FullName,
                        Name = fi.Name,
                        Path = fi.Directory.FullName,
                        DateTaken = dt,
                        CameraMake = make,
                        CameraModel = model,
                        Size = fi.Length,
                        DateCreated = fi.CreationTime,
                        DateModified = fi.LastWriteTime,
                        DateAccessed = fi.LastAccessTime
                    };
                }
            }
            catch { }
            return photo;
        }
    }
}
