using CrisesControl.SharedKernel.Enums;
using System;
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
    public static string ToLTString(this LogoType logoType)
    {
        return logoType switch
        {
            LogoType.EMAILLOGO => "Email Logo",
            LogoType.PHONELOGO => "PHONE LOGO",
          
            _ => throw new ArgumentOutOfRangeException(nameof(logoType), logoType, null)
        };
    }
    public static string PWDencrypt(this string strPwdString)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        // Convert the input string to a byte array and compute the hash.
        byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(strPwdString));

        StringBuilder sBuilder = new StringBuilder();
        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        // Return the hexadecimal string.
        return sBuilder.ToString();
    }
    public static string Truncate(string value, int maxChars)
    {
        return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
    }
    public static string FormatMobile(string ISD, string Mobile)
    {
        if (!string.IsNullOrEmpty(Mobile))
        {
            Mobile = Mobile.TrimStart('0').TrimStart('+');
            if (Mobile.Length > 4)
            {
                ISD = ISD.TrimStart('+').TrimStart('0');
                return "+" + ISD + Mobile;
            }
            else
            {
                return "";
            }
        }
        else
        {
            return "";
        }
    }
    public static string SectoTime( int secs)
    {
        int hours = secs / 3600;
        int mins = (secs % 3600) / 60;
        secs %= 60;
        return string.Format("{0:D2}h:{1:D2}m:{2:D2}s", hours, mins, secs);
    }
   
}