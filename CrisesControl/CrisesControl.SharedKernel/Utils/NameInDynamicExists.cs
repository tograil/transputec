using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CrisesControl.SharedKernel.Utils
{
    public static class NameInDynamicExists
    {
        public static bool NameExists(dynamic value, string name)
        {
            Type x = value.GetType();
            return x.GetProperties().Any(p => p.Name.Equals(name));
        }
    }
}