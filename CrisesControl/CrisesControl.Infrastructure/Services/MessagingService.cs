using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Core.Queues;
using CrisesControl.Infrastructure.Client;

namespace CrisesControl.Infrastructure.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly MessageSendService.MessageSendServiceClient _messageSendServiceClient;
        private readonly IDBCommonRepository _dbCommonRepository;

        public MessagingService(MessageSendService.MessageSendServiceClient messageSendServiceClient,
            IDBCommonRepository dbCommonRepository)
        {
            _messageSendServiceClient = messageSendServiceClient;
            _dbCommonRepository = dbCommonRepository;
        }

        public async Task EnqueueMessage(ICollection<MessageQueueItem> messages)
        {
            var sublists = SplitToSublists(messages);

            foreach (var sublist in sublists)
            {
                var input = new AddMessageQuery
                {
                    MessageTemplate = sublist.First().MessageText,
                    MessageAttchment = string.Empty,
                    MessageGuid = Guid.NewGuid().ToString(),
                    Recipients = { }
                };

                foreach (var record in sublist)
                {
                    var ackUrl = await _dbCommonRepository.GetCompanyParameter("ONE_CLICK_EMAIL_ACKNOWLEDGE", record.CompanyId);

                    input.Recipients.Add(new Recipient
                    {
                        Email = new Email
                        {
                            Address = record.UserEmail,
                            RecipientName = $"{record.FirstName} {record.LastName}",
                            AckUrl = ackUrl
                        },
                        Devices = { },
                        Phone = new PhoneCall
                        {
                            Enabled = false
                        },
                        Sms = new SmsMessage
                        {
                            Enabled = false
                        },
                        IdGuid = Guid.NewGuid().ToString()
                    });
                }

                var reply = await _messageSendServiceClient.SetMessageAsync(input);
            }

            
        }

        public List<List<T>> SplitToSublists<T>(IEnumerable<T> source)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 500)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}