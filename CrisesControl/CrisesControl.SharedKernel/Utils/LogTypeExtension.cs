using CrisesControl.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.SharedKernel.Utils
{
    public static class LogTypeExtension
    {
        public static string ToLGString(this LogType logType)
        {
            return logType switch
            {
                LogType.PHONE => "PHONE",
                LogType.TEXT => "TEXT",
                LogType.RECORDING => "RECORDING",
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
        }
    }
}
