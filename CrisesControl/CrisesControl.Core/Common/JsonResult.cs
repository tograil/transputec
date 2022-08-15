using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Common
{
    public class JsonResult
    {
        public string Result { get; set; }
        public int UserId { get; set; }
        public int LocationId { get; set; }
        public int GroupId { get; set; }
        public int UserSecurityGroupId { get; set; }
        public int Success { get; set; }
        public int ResultId { get; set; }
    }
}
