
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Jobs;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Core.Queues.Services;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services {
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class CascadeMessageJob : IJob {
        private readonly CrisesControlContext _controlContext;
        private readonly IQueueMessageService _queueHelper;
        private readonly IMessageService _MSG;
        private readonly IDBCommonRepository _DBC;
        public CascadeMessageJob(CrisesControlContext controlContext, IDBCommonRepository DBC, IMessageService MSG, IQueueMessageService queue)
        {
            this._controlContext = controlContext;
            this._DBC = DBC;
            this._MSG = MSG;
            this._queueHelper = queue;
        }
        //DBCommon DBC = new DBCommon();

        public Task Execute(IJobExecutionContext context) {
            int MessageID = context.JobDetail.JobDataMap.GetInt("MessageId");
            int Priority = context.JobDetail.JobDataMap.GetInt("Priority");
            string MessageType = context.JobDetail.JobDataMap.GetString("MessageType");

            try {


                var pMessageID = new SqlParameter("@MessageID", MessageID);
                var pPriority = new SqlParameter("@Priority", Priority);
                var pMessageType = new SqlParameter("@MessageType", MessageType);

                int RowsCount = _controlContext.Database.ExecuteSqlRaw("Pro_Create_Message_Queue_Cascading @MessageID, @MessageType, @Priority", pMessageID, pMessageType, pPriority);

                if (RowsCount > 0) {
                    _queueHelper.MessageDevicePublish(MessageID, Priority);
                }

            } catch (Exception ex) {
                throw ex;
            }
            return Task.WhenAll();
        }
    }
    public class SOSCascadeMessageJob : IJob {
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDBCommonRepository DBC;
        public SOSCascadeMessageJob(CrisesControlContext controlContext, IHttpContextAccessor httpContextAccessor, IDBCommonRepository _DBC)
        {
            this._context = controlContext;
            this._httpContextAccessor = httpContextAccessor;
            DBC = _DBC;
        }
        
        public async Task Execute(IJobExecutionContext context)
        {

            try {
                int CompanyId = context.JobDetail.JobDataMap.GetInt("CompanyID");
                int MessageID = context.JobDetail.JobDataMap.GetInt("MessageID");



                var pMessageID = new SqlParameter("@MessageID", MessageID);
                var users = _context.Set<UnAckUsers>().FromSqlRaw("exec Pro_Get_Unack_User @MessageID", pMessageID).ToList();

                    string APIBaseURL =await  DBC.LookupWithKey("APIBASEURL");
                    foreach (var user in users)
                    {
                        LaunchSOS(CompanyId, user.UserId, APIBaseURL);
                    }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            await Task.WhenAll();
        }

        public void LaunchSOS(int CompanyId, int UserId, string BaseURL) {
            try {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(BaseURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                LaunchSOSAny IP = new LaunchSOSAny();
                IP.CompanyId = CompanyId;
                IP.UserId = UserId;
                IP.CallBackMethod = 3;

                try {
                    HttpResponseMessage response = client.PostAsJsonAsync("App/LaunchSOSAny", IP).Result;
                    response.EnsureSuccessStatusCode();

                    Task<string> resultString = response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode) {
                        DBC.CreateLog("ERROR", "Failed Scheduler SOS" + resultString.Result, null, "CascadeSOSMessageJob", "CascadeSOSMessageJob", 0);
                    }

                } catch (HttpRequestException ex) {
                    throw ex;
                }
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}
