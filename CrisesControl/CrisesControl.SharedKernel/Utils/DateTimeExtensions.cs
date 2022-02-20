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
}