using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.SopLibrary
{
    public class SopSection
    {
        public string? Name { get; set; }
        public string? IncidentTypeName { get; set; }
        public int? LibSopheaderId { get; set; }
        public int? IncidentId { get; set; }
        public DateTimeOffset ReviewDate { get; set; }
        public string? Sopversion { get; set; }
        public int? LibSectionId { get; set; }
        public string? SectionDescription { get; set; }
        public int? LibContentId { get; set; }
        public string? SectionName { get; set; }
        public List<LibContentTag>? TagId { get; set; }
    }
}
