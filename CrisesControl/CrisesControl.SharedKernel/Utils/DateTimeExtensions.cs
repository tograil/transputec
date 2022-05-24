using System;

namespace CrisesControl.SharedKernel.Utils;

public static class DateTimeExtensions
{
    public static DateTimeOffset GetDateTimeOffset(this DateTime crTime, string timeZoneId = "GMT Standard Time")
    {
        switch (crTime.Year)
        {
            case <= 2000:
                return crTime;
            case > 3000:
                crTime = DateTime.MaxValue.AddHours(-48);
                break;
        }

        var cet = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var offset = cet.GetUtcOffset(crTime);

        var newvals = new DateTimeOffset(new DateTime(crTime.Year, crTime.Month, crTime.Day, crTime.Hour, crTime.Minute, crTime.Second, crTime.Millisecond));

        var convertedtime = newvals.ToOffset(offset);

        return convertedtime;
    }
    public static DateTime GetLocalTime(string TimeZoneId, DateTime? ParamTime = null)
    {
        try
        {
            if (string.IsNullOrEmpty(TimeZoneId))
                TimeZoneId = "GMT Standard Time";

            DateTime retDate = DateTime.Now.ToUniversalTime();

            DateTime dateTimeToConvert = new DateTime(retDate.Ticks, DateTimeKind.Unspecified);

            DateTime timeUtc = DateTime.UtcNow;

            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            retDate = TimeZoneInfo.ConvertTimeFromUtc(dateTimeToConvert, cstZone);

            return retDate;
        }
        catch (Exception ex) { throw ex; }
        return DateTime.Now;
    }
    public static DateTime DbDate()
    {
        return new DateTime(1900, 01, 01, 0, 0, 0);
    }
}