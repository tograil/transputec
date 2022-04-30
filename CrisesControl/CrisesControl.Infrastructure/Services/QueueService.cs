using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.Queues;
using CrisesControl.Core.Queues.Services;
using CrisesControl.Core.Settings.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CrisesControl.Infrastructure.Services;

public class QueueService : IQueueService
{
    private readonly CrisesControlContext _context;
    private readonly ILogger<QueueService> _logger;
    private readonly ISettingsRepository _settingsRepository;

    public QueueService(CrisesControlContext context,
        ILogger<QueueService> logger,
        ISettingsRepository settingsRepository)
    {
        _context = context;
        _logger = logger;
        _settingsRepository = settingsRepository;
    }

    public async Task<ICollection<MessageQueueItem>> GetDeviceQueue(int messageId, MessageMethod method, int messageDeviceId = 0, int priority = 1)
    {
        var pMessageId = new SqlParameter("@MessageID", messageId);
        var pMethod = new SqlParameter("@Method", method.ToDbString());
        var pMessageDeviceId = new SqlParameter("@MessageDeviceID", messageDeviceId);
        var pPriority = new SqlParameter("@Priority", priority);

        var messageQueue = await _context.Set<MessageQueueItem>()
            .FromSqlRaw("Pro_Get_Message_Device_Queue {0}, {1}, {2}, {3}",
                pMessageId, pMethod, pMessageDeviceId, pPriority)
            .ToListAsync();

        messageQueue = messageQueue.Select(x =>
        {
            x.SenderName = string.Join(" ", x.SenderFirstName, x.SenderLastName);
            if (method == MessageMethod.Email)
            {
                x.DeviceAddress = x.UserEmail;
            }

            return x;
        }).ToList();

        _logger.LogInformation($"Queue count for {method.ToDbString()} is {messageQueue.Count}");

        return messageQueue;
    }

    public Task PublishMessage(int messageId, int priority, MessageMethod method = MessageMethod.All)
    {
        var rabbitHosts = RabbitHosts();

        throw new System.NotImplementedException();
    }

    public async Task<bool> EnqueueMessage(int messageId, ICollection<string> rabbitHosts, MessageMethod method)
    {
        var rabbitUser = _settingsRepository.GetSetting("RABBITMQ_USER");
        var rabbitPassword = _settingsRepository.GetSetting("RABBITMQ_PASSWORD");

        var deviceList = await GetDeviceQueue(messageId, method);

        var rabbitFactory = new ConnectionFactory
        {
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
            NetworkRecoveryInterval = new TimeSpan(0, 0, 15),
            RequestedHeartbeat = new TimeSpan(0, 0, 15),
            UserName = rabbitUser,
            Password = rabbitPassword
        };

        using var connection = rabbitFactory.CreateConnection(rabbitHosts.ToList());
        using var model = connection.CreateModel();

        var exchangeName = "cc_processing_exchange";

        model.ExchangeDeclare(exchangeName, "direct", true, true);

        throw new System.NotImplementedException();
    }

    public ICollection<string> RabbitHosts()
    {
        var hosts = _settingsRepository.GetSetting("RABBITMQ_HOST");

        return hosts.Split(",");
    }
}