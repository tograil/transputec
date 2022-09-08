using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.App
{
    public class AppIconURL
    {
        public string LangCode { get; set; }
        public string IconURL { get; set; }
        public string Platform { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
    }
}
