namespace CrisesControl.Core.Settings.Repositories;

public interface ISettingsRepository
{
    public string GetSetting(string key, string defaultValue = "");
}