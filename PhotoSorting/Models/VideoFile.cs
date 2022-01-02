using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSorting.Models
{
    public class VideoFile
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public int Length { get; set; } // Length of video in ms
        public DateTime? DateCreated { get; set; } = null;
        public DateTime? DateModified { get; set; } = null;
        public bool ToDelete { get; set; } = false;
        public bool Deleted { get; set; } = false;
        public int? DoubleSetId { get; set; }


        public static readonly string[] VIDEO_EXT = new string[]
        {
            ".avi",
            ".mp4",
            ".wmv",
            "3gp",
            "m4v"
        };

        public static async Task<VideoFile> GetVideo(FileInfo fi)
        {
            await Task.Delay(1);
            if (!VIDEO_EXT.Contains(fi.Extension.ToLower()))
            {
                throw new Exception($"{fi.Extension} is not a video file!");
            }
            VideoFile video = null;
            string path = fi.FullName;
            try
            {
                video = new VideoFile
                {
                    FullName = fi.FullName,
                    Name = fi.Name,
                    Path = fi.Directory.FullName,
                    Size = fi.Length,
                    DateCreated = fi.CreationTime,
                    DateModified = fi.LastWriteTime
                };
            }
            catch (Exception ex)
            { }

            return video;
        }
    }
}
