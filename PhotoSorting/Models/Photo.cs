using System;
using System.Collections.Generic;
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
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime DateTaken { get; set; }
        public string CameraMake { get; set; }
        public string CameraModel { get; set; }
        public long Size { get; set; }

        public Photo()
        {
        }

        //we init this once so that if the function is repeatedly called
        //it isn't stressing the garbage man
        private static Regex r = new Regex(":");

        //retrieves the datetime WITHOUT loading the whole image
        public static async Task<Photo> GetPhoto(FileInfo fi)
        {
            if (fi.Extension.ToLower() != ".jpg")
            {
                throw new Exception("Not a jpeg image!");
            }
            Photo photo = null;
            string path = fi.FullName;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = await Task.Run(() => myImage.GetPropertyItem(PropertyTags.DATE_TAKEN)); //36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    var dt = DateTime.Parse(dateTaken);

                    propItem = myImage.GetPropertyItem(PropertyTags.CAMERA_MAKE);
                    var make = Encoding.UTF8.GetString(propItem.Value);

                    propItem = myImage.GetPropertyItem(PropertyTags.CAMERA_MODEL);
                    var model = Encoding.UTF8.GetString(propItem.Value);

                    photo = new Photo
                    {
                        FullName = fi.FullName,
                        Name = fi.Name,
                        Path = fi.Directory.FullName,
                        DateTaken = dt,
                        CameraMake = make,
                        CameraModel = model,
                        Size = fi.Length
                    };
                }
            }
            catch { }
            return photo;
        }
    }
}
