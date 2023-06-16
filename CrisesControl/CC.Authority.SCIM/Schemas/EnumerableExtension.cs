//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Schemas
{
    internal static class EnumerableExtension
    {
        public static int CheckedCount<T>(this IEnumerable<T> enumeration)
        {
            long longCount = enumeration.LongCount();
            int result = Convert.ToInt32(longCount);
            return result;
        }
    }
}
