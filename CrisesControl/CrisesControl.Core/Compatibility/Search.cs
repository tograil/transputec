using System;

namespace CrisesControl.Core.Compatibility
{
    [Obsolete("Added for compatibility with old portal")]
    public class Search
    {
        public string Value { get; set; }
        public string Regex { get; set; }
    }
}
