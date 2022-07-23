using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.SopLibrary
{
    public class SopSectionList
    {
        public LibSopheader Header { get; set; }
        public int IncidentID { get; set; }
        public string Name { get; set; }
        public string IncidentTypeName { get; set; }
        public string SectionName { get; set; }
        public int Status { get; set; }

       
    }
}
