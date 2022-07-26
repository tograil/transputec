using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Academy
{
    public class AcademyVideos
    {
        public int VideoID { get; set; }
        public string VideoKey { get; set; }
        public string VideoTitle { get; set; }
        public string VideoDescription { get; set; }
        public string SourceURL { get; set; }
        public string SourceType { get; set; }
        public string VideoImage { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public bool CloseOnEsc { get; set; }
        public int Status { get; set; }
    }
}
