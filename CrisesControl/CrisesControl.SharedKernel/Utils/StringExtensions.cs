﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CrisesControl.SharedKernel.Utils;

public static class StringExtensions
{
    public static string Left(this string str, int lngth, int stpoint = 0)
    {
        return str.Substring(stpoint, Math.Min(str.Length, lngth));
    }

    public static string FixMobileZero(this string strNumber)
    {
        strNumber = Regex.Replace(strNumber, @"\D", string.Empty);
        strNumber = strNumber.Left(1) == "0" ? Left(strNumber, strNumber.Length - 1, 1) : strNumber;
        return strNumber;
    }

    public static string PwdEncrypt(this string strPwdString)
    {
        var md5 = MD5.Create();
        // Convert the input string to a byte array and compute the hash.
        var data = md5.ComputeHash(Encoding.UTF8.GetBytes(strPwdString));

        var sBuilder = new StringBuilder();
        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        foreach (var t in data)
        {
            sBuilder.Append(t.ToString("x2"));
        }
        // Return the hexadecimal string.
        return sBuilder.ToString();
    }
}