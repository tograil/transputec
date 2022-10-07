using CrisesControl.Core.Compatibility.Jobs;
using CrisesControl.Core.Import;
using CrisesControl.Core.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages.Services
{
    public interface IPingService
    {
        Task<List<PublicAlertRtn>> GetPublicAlert(int companyId, int targetUserId);
        Task<dynamic> GetPublicAlertTemplate(int templateId, int userId, int companyId);
        Task<int> PingMessages(int companyId, string messageText, List<AckOption> ackOptions, int priority, bool multiResponse, string messageType,
       int incidentActivationId, int currentUserId, string timeZoneId, PingMessageObjLst[] pingMessageObjLst, int[] usersToNotify, int assetId = 0,
       bool silentMessage = false, int[] messageMethod = null, List<MediaAttachment> mediaAttachments = null, List<string> socialHandle = null,
       int cascadePlanID = 0);
        Task<dynamic> ProcessPAFile(string userListFile, bool hasHeader, int emailColIndex, int phoneColIndex, int postcodeColIndex, int latColIndex, int longColIndex, string sessionId);
        Task<CommonDTO> ResendFailure(int messageId, string commsMethod);
        Task<dynamic> SendPublicAlert(string messageText, int[] messageMethod, bool schedulePA, DateTime scheduleAt, string sessionId, int userId, int companyId, string timeZoneId);
        Task<int> CreatePublicAlert(int messageId, bool scheduled, DateTimeOffset scheduleAt, int userId, string timeZoneId);
        Task<dynamic> ReplyToMessage(int parentId, string messageText, string replyTo, string messageType, int activeIncidentId, int[] messageMethod,
            int cascadePlanId, int currentUserId, int companyId, string timeZoneId);
    }
}
