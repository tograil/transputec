using CrisesControl.Core.Queues.Services;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services.Jobs {
    public class MessageDevicePublishJob : IJob {
        private readonly IQueueMessageService _queueHelper;
        public MessageDevicePublishJob( IQueueMessageService queueHelper) {
            Debug.WriteLine("Inizialing the MessageDeviceQueueJob");
            try {
                _queueHelper = queueHelper;
            } catch (Exception) {

                throw;
            }
        }

        public async Task Execute(IJobExecutionContext context) {
            Console.WriteLine("Executing the MessageDevicePublishJob");
            try {

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
        private readonly IQueueMessageService _queueHelper;
        public MessageDeviceQueueJob(IQueueMessageService queueHelper) {
            Debug.WriteLine("Inizialing the MessageDeviceQueueJob");
            try {
                _queueHelper = queueHelper;
            } catch (Exception) {

                throw;
            }

        }

        public async Task Execute(IJobExecutionContext context) {
            Debug.WriteLine("Executing the MessageDeviceQueueJob");
            try {

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
        private readonly IQueueMessageService _queueHelper;
        public PublishMessageQueueJob(IQueueMessageService queueHelper) {
            Debug.WriteLine("Inizialing the MessageDeviceQueueJob");
            try {
                _queueHelper = queueHelper;
            } catch (Exception) {

                throw;
            }

        }

        public async Task Execute(IJobExecutionContext context) {
            Debug.WriteLine("Executing the MessageDeviceQueueJob");
            try {

                int MessageID = context.JobDetail.JobDataMap.GetInt("MessageID");
                string? Method = context.JobDetail.JobDataMap.GetString("Method");
                int Priority = context.JobDetail.JobDataMap.GetInt("Priority");

                var rabbithosts = await _queueHelper.RabbitHosts();

                await _queueHelper.PublishMessageQueue(MessageID, rabbithosts, Method, null, Priority);

            } catch (Exception) {
                throw;
            }

            //return Task.WhenAll();
        }
    }

}
