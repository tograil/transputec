using CrisesControl.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.SharedKernel.Utils
{
    public static class UpateTypeExtensions
    {
        public static string ToUString(this UpdateType updateType)
        {
            return updateType switch
            {
                UpdateType.DNDON => "DNDON",
                UpdateType.LANGUAGE => "LANGUAGE",
                UpdateType.SIRENON => "SIRENON",
                UpdateType.SOUND=>"SOUND",
                _ => throw new ArgumentOutOfRangeException(nameof(updateType), updateType, null)
            };
        }
    }
}
