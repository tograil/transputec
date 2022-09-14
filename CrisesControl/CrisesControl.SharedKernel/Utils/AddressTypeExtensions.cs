using CrisesControl.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.SharedKernel.Utils
{
    public static class  AddressTypeExtensions
    {
        public static string ToAdString(this AddressType addressType)
        {
            return addressType switch
            {
                AddressType.Primary => "Primary",
                AddressType.Billing => "Billing",
                AddressType.Other => "Other",
                 _ => throw new ArgumentOutOfRangeException(nameof(addressType), addressType, null)
            };
        }
    }
}
