using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSorting.Models
{
    public class DoubleSet
    {
        public int Id { get; set; }
        public int? Preferred { get; set; }
        public IList<Photo> Photos { get; set; } = new List<Photo>();
        public IList<VideoFile> VideoFiles { get; set; } = new List<VideoFile>();
    }
}
