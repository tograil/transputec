using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services.Jobs {
    public class MessageDevicePublishJob : IJob {
        private readonly IServiceProvider _serviceProvider;
        public MessageDevicePublishJob(IServiceProvider serviceProvider) {
            Debug.WriteLine("Inizialing the MessageDeviceQueueJob");
            try {
                _serviceProvider = serviceProvider;
            } catch (Exception) {

                throw;
            }
        }

        public async Task Execute(IJobExecutionContext context) {
            Console.WriteLine("Executing the MessageDevicePublishJob");
            try {
                var _db = _serviceProvider.GetRequiredService<CrisesControlContext>();
                var _httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
                QueueHelper _queueHelper = new QueueHelper(_db, _httpContextAccessor);


                int MessageID = context.JobDetail.JobDataMap.GetInt("MessageID");
                int Priority = context.JobDetail.JobDataMap.GetInt("Priority");
                string? Method = context.JobDetail.JobDataMap.GetString("Method");

                await _queueHelper.MessageDevicePublish(MessageID, Priority, Method);

            } catch (Exception) {

                throw;
            }

        }
    }

    public class MessageDeviceQueueJob : IJob {
        private readonly IServiceProvider _serviceProvider;
        public MessageDeviceQueueJob(IServiceProvider serviceProvider) {
            Debug.WriteLine("Inizialing the MessageDeviceQueueJob");
            try {
                _serviceProvider = serviceProvider;
            } catch (Exception) {

                throw;
            }

        }

        public async Task Execute(IJobExecutionContext context) {
            Debug.WriteLine("Executing the MessageDeviceQueueJob");
            try {

                var _db = _serviceProvider.GetRequiredService<CrisesControlContext>();
                var _httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
                QueueHelper _queueHelper = new QueueHelper(_db, _httpContextAccessor);

                int MessageID = context.JobDetail.JobDataMap.GetInt("MessageID");
                string? MessageType = context.JobDetail.JobDataMap.GetString("MessageType");
                int Priority = context.JobDetail.JobDataMap.GetInt("Priority");
                int CascadePlanID = context.JobDetail.JobDataMap.GetInt("CascadePlanID");

                await _queueHelper.MessageDeviceQueue(MessageID, MessageType, Priority, CascadePlanID);
            } catch (Exception) {
                throw;
            }

            //return Task.WhenAll();
        }
    }

    public class PublishMessageQueueJob : IJob {
        private readonly IServiceProvider _serviceProvider;
        public PublishMessageQueueJob(IServiceProvider serviceProvider) {
            Debug.WriteLine("Inizialing the MessageDeviceQueueJob");
            try {
                _serviceProvider = serviceProvider;
            } catch (Exception) {

                throw;
            }

        }

        public async Task Execute(IJobExecutionContext context) {
            Debug.WriteLine("Executing the MessageDeviceQueueJob");
            try {

                var _db = _serviceProvider.GetRequiredService<CrisesControlContext>();
                var _httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
                QueueHelper _queueHelper = new QueueHelper(_db, _httpContextAccessor);

                int MessageID = context.JobDetail.JobDataMap.GetInt("MessageID");
                string? Method = context.JobDetail.JobDataMap.GetString("Method");
                int Priority = context.JobDetail.JobDataMap.GetInt("Priority");

                var rabbithosts = _queueHelper.RabbitHosts();

                await _queueHelper.PublishMessageQueue(MessageID, rabbithosts, Method, null, Priority);

            } catch (Exception) {
                throw;
            }

            //return Task.WhenAll();
        }
    }

}
