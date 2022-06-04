using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CrisesControl.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly CrisesControlContext _context;

    public MessageService(IMessageRepository messageRepository, CrisesControlContext context)
    {
        _messageRepository = messageRepository;
        _context = context;   
    }

    public Task ProcessMessageMethod(int messageId, int[] messageMethod, int incidentActivationId, bool trackUser = false)
    {
        throw new System.NotImplementedException();
    }

}