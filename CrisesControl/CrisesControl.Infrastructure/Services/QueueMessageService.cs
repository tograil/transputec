using System;
using AutoMapper;
using CrisesControl.Core.Queues;
using CrisesControl.Core.Queues.Services;
using CrisesControl.Core.Settings.Repositories;

namespace CrisesControl.Infrastructure.Services;

public class QueueMessageService : IQueueMessageService
{
    private readonly IMapper _mapper;
    private readonly ISettingsRepository _settingsRepository;

    public QueueMessageService(IMapper mapper,
        ISettingsRepository settingsRepository)
    {
        _mapper = mapper;
        _settingsRepository = settingsRepository;
    }

    public T GetMessage<T>(MessageQueueItem baseMessage) where T : MessageQueueItem
    {
        var initialValue = _mapper.Map<T>(baseMessage);

        switch (initialValue)
        {
            case TextMessage textMessage:
                FillText(ref textMessage);
                break;
            case EmailMessage emailMessage:
                FillEmail(ref emailMessage);
                break;
            case PushMessage pushMessage:
                FillPush(ref pushMessage);
                break;
            case PhoneMessage phoneMessage:
                FillPhone(ref phoneMessage);
                break;
        }

        return initialValue;
    }

    private void FillText(ref TextMessage baseText)
    {
        baseText.TaskURL = _settingsRepository.GetSetting("INCIDENT_TASK_URL");
        baseText.TaskURLLabel = _settingsRepository.GetSetting("TASK_URL_LABEL");

        baseText.UseCopilot = Convert.ToBoolean(_settingsRepository.GetSetting("USE_MESSAGING_COPILOT"));
    }

    private void FillEmail(ref EmailMessage baseEmail)
    {
        baseEmail.TwitterPage = _settingsRepository.GetSetting("CC_TWITTER_PAGE");
        baseEmail.TwitterIcon = _settingsRepository.GetSetting("CC_TWITTER_ICON");
        baseEmail.FacebookPage = _settingsRepository.GetSetting("CC_FB_PAGE");
        baseEmail.FacebookIcon = _settingsRepository.GetSetting("CC_FB_ICON");
        baseEmail.LinkedInPage = _settingsRepository.GetSetting("CC_LINKEDIN_PAGE");
        baseEmail.LinkedInIcon = _settingsRepository.GetSetting("CC_LINKEDIN_ICON");
        baseEmail.Domain = _settingsRepository.GetSetting("DOMAIN");
        baseEmail.PortalURL = _settingsRepository.GetSetting("PORTAL");
        baseEmail.ACKUrl = _settingsRepository.GetSetting("ACKNOWLEDGE_URL");
        baseEmail.SupportEmail = _settingsRepository.GetSetting("CC_USER_SUPPORT_LINK");
        baseEmail.CCLogo = _settingsRepository.GetSetting("CCLOGO");
        baseEmail.TemplatePath = _settingsRepository.GetSetting("COMMS_TEMPLATE_PATH");
        baseEmail.EmailFrom = _settingsRepository.GetSetting("ALERT_EMAILFROM");
        baseEmail.SMTPHost = _settingsRepository.GetSetting("SMTPHOST");
        baseEmail.EmailSub = _settingsRepository.GetSetting("EMAILSUB");
        baseEmail.TaskURL = _settingsRepository.GetSetting("INCIDENT_TASK_URL");
        baseEmail.TaskURLLabel = _settingsRepository.GetSetting("TASK_URL_LABEL");
        baseEmail.SendGridAPIKey = _settingsRepository.GetSetting("SEND_GRID_API_KEY");
        baseEmail.AwsSesAccessKey = _settingsRepository.GetSetting("AWS_SES_ACCCESS_KEY");
        baseEmail.AWSSESSecretKey = _settingsRepository.GetSetting("AWS_SES_SECRET_KEY");
        baseEmail.Office365Host = _settingsRepository.GetSetting("OFFICE365_HOST");
    }

    private void FillPush(ref PushMessage basePush)
    {
        basePush.AppleCertPwd = _settingsRepository.GetSetting("APPLECERPWD");
        basePush.AppleCertPath = _settingsRepository.GetSetting("APPLECERTPATH");
        basePush.ApplePushMode = Convert.ToBoolean(_settingsRepository.GetSetting("APPLEPUSHMODE"));
        basePush.GoogleApiKey = _settingsRepository.GetSetting("GOOGLE_API_KEY");
        basePush.SoundFileName = _settingsRepository.GetSetting("PUSH_SOUND_FILE");
        basePush.WinPackageName = _settingsRepository.GetSetting("WINAPP_PACKAGE_NAME");
        basePush.WinAppSID = _settingsRepository.GetSetting("WINAPP_SID");
        basePush.WinClientSecret = _settingsRepository.GetSetting("WINAPP_CLIENT_SECRET");
        basePush.WinDeskPackageName = _settingsRepository.GetSetting("WINDESK_PACKAGE_NAME");
        basePush.WinAppSID = _settingsRepository.GetSetting("WINDESK_SID");
        basePush.WinClientSecret = _settingsRepository.GetSetting("WINDESK_CLIENT_SECRET");
    }

    private void FillPhone(ref PhoneMessage basePhone)
    {
        //TODO: Nothing to fill. Find
    }
}