using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class Api
    {
        public int ApiId
        {
            get;
            set;
        }

        public string ApiUrl
        {
            get;
            set;
        }

        public string ApiHost
        {
            get;
            set;
        }

        public bool IsCurrent
        {
            get;
            set;
        }

        public int Status
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public string AppVersion
        {
            get;
            set;
        }

        public string ApiMode
        {
            get;
            set;
        }

        public string Platform
        {
            get;
            set;
        }
    }
}
