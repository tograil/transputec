using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.SharedKernel.Utils
{
    public static class CCConstants
    {
        public static Dictionary<string, string> GlobalVars = new Dictionary<string, string>();

        public static string CMSMSStatus(int Sts)
        {
            string Status = "";
            switch (Sts)
            {
                case 19:
                case 37:
                    Status = "sent";
                    break;
                case 20:
                    Status = "delivered";
                    break;
                case 21:
                    Status = "failed";
                    break;
                case 22:
                case 40:
                    Status = "undelivered";
                    break;
            }
            return Status;
        }

    }
}
