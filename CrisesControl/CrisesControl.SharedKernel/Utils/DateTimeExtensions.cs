using System;
using System.Threading.Tasks;

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
    public static void GetStartEndDate(bool IsThisWeek, bool IsThisMonth, bool IsLastMonth, ref DateTime stDate, ref DateTime enDate, DateTimeOffset StartDate, DateTimeOffset EndDate)
    {
        if (IsThisWeek)
        {
            int dayofweek = Convert.ToInt32(DateTime.Now.DayOfWeek);
            stDate = DateTime.Now.AddDays(0 - dayofweek);
            enDate = DateTime.Now.AddDays(7 - dayofweek);
            stDate = new DateTime(stDate.Year, stDate.Month, stDate.Day, 0, 0, 0);
            enDate = new DateTime(enDate.Year, enDate.Month, enDate.Day, 23, 59, 59);
        }
        else if (IsThisMonth)
        {
            stDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            enDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 23, 59, 59);
        }
        else if (IsLastMonth)
        {
            DateTime currentDate = DateTime.Now;
            int year = currentDate.Year;
            int month = currentDate.Month;

            if (month == 1)
            {
                year = year - 1;
                month = 12;
            }
            else
            {
                month = month - 1;
            }
            stDate = new DateTime(year, month, 1, 0, 0, 0);
            enDate = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);
        }
        else
        {
            stDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0);
            enDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 23, 59, 59);
        }
    }
}