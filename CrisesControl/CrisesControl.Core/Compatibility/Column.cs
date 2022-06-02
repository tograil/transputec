using System;

namespace CrisesControl.Core.Compatibility
{
    [Obsolete("Added for compatibility with old portal")]
    public class Column
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public Search Search { get; set; }
    }
}
