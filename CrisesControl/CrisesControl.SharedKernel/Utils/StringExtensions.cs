using System;

namespace CrisesControl.SharedKernel.Utils;

public static class StringExtensions
{
    public static string Left(this string str, int lngth, int stpoint = 0)
    {
        return str.Substring(stpoint, Math.Min(str.Length, lngth));
    }
}