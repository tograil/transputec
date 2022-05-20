using System;
using CrisesControl.SharedKernel.Enums;

namespace CrisesControl.Core.Compatibility;

public static class JobCommandConstants
{
    public const string InitiateIncident = "Incident/InitiateCompanyIncident";
    public const string InitiateAndLaunchIncident = "Incident/IncidentInitiateAndLaunch";
    public const string PingMessage = "Messaging/PingMessage";

    public const string OneTimeFrequency = "ONE TIME";
    public const string DailyFrequency = "DAILY";
    public const string WeeklyFrequency = "WEEKLY";
    public const string MonthlyFrequency = "MONTHLY";

    public const string SubTypeOnce = "ONCE";
    public const string SubTypeHourly = "HOUR";
    public const string SubTypeMinute = "MINUTE";

    public static JobType ParseCommand(string jobCommandString)
    {
        return jobCommandString switch
        {
            InitiateIncident => JobType.InitiateIncident,
            InitiateAndLaunchIncident => JobType.InitiateAndLaunchIncident,
            PingMessage => JobType.PingMessage,
            _ => JobType.Unknown
        };
    }

    public static JobFrequencyType ParseFrequencyType(string jobFrequency)
    {
        return jobFrequency.ToUpper() switch
        {
            OneTimeFrequency => JobFrequencyType.OneTime,
            DailyFrequency => JobFrequencyType.Daily,
            WeeklyFrequency => JobFrequencyType.Weekly,
            MonthlyFrequency => JobFrequencyType.Monthly,
            _ => throw new ArgumentOutOfRangeException(jobFrequency)
        };
    }

    public static JobSubDayType ParseJobSubDayType(string jobSubDayType)
    {
        return jobSubDayType.ToUpper() switch
        {
            SubTypeOnce => JobSubDayType.Once,
            SubTypeHourly => JobSubDayType.Hour,
            SubTypeMinute => JobSubDayType.Minute,
            _ => throw new ArgumentOutOfRangeException(jobSubDayType)
        };
    }
}