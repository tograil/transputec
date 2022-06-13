using System;

namespace CrisesControl.Core.Compatibility
{
    [Obsolete("Added for compatibility with old portal")]
    public class Order
    {
        public string Column { get; set; }
        public string Dir { get; set; }
    }
}
