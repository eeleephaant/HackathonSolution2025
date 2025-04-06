using ChatService.DBModels;
using ChatService.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StartupService.Models;

namespace ChatService.Services;

public class ChatHub(HackatonContext _context, NotificationProducer _notificationProducer) : Hub
{
    public async Task JoinStartupRoom(int startupId, int userId)
    {
        var isMember = await _context.Startups
            .Where(s => s.Id == startupId)
            .AnyAsync(s =>
                s.OwnerId == userId ||
                s.StartupWorkers.Any(w => w.UserId == userId) ||
                s.StartupScientists.Any(sci => sci.UserId == userId) ||
                s.StartupInvestors.Any(inv => inv.InvestorId == userId)
            );

        if (!isMember)
        {
            await Clients.Caller.SendAsync("AccessDenied", "Вы не состоите в этом стартапе.");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"startup-{startupId}");
    }

    public async Task LeaveStartupRoom(int startupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"startup-{startupId}");
    }

    public async Task SendMessageToStartup(int startupId, MessageDTO messageDto)
    {
        var isMember = await _context.Startups
            .Where(s => s.Id == startupId)
            .AnyAsync(s =>
                s.OwnerId == messageDto.Sender.Id ||
                s.StartupWorkers.Any(w => w.UserId == messageDto.Sender.Id) ||
                s.StartupScientists.Any(sci => sci.UserId == messageDto.Sender.Id) ||
                s.StartupInvestors.Any(inv => inv.InvestorId == messageDto.Sender.Id)
            );

        if (!isMember)
        {
            await Clients.Caller.SendAsync("AccessDenied", "Вы не состоите в этом стартапе.");
            return;
        }

        var message = new Message
        {
            StartupId = startupId,
            SenderId = messageDto.SenderId,
            Text = messageDto.Text,
            AttachmentUrl = messageDto.AttachmentUrl,
            AttachmentType = messageDto.AttachmentType,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var sender = await _context.Users.FindAsync(messageDto.SenderId);

        var dto = new MessageDTO
        {
            Id = message.Id,
            StartupId = startupId,
            SenderId = messageDto.SenderId,
            Text = messageDto.Text,
            AttachmentUrl = messageDto.AttachmentUrl,
            AttachmentType = messageDto.AttachmentType,
            SentAt = message.SentAt,
            Sender = new UserDTO
            {
                Id = sender.Id,
                FirstName = sender.FirstName,
                LastName = sender.LastName,
                ThirdName = sender.ThirdName
            }
        };

        var startup = await _context.Startups
            .Where(s => s.Id == startupId)
            .Include(s => s.StartupInvestors)
            .ThenInclude(si => si.Investor)
            .Include(st => st.StartupScientists)
            .ThenInclude(si => si.User)
            .Include(st => st.StartupWorkers)
            .ThenInclude(stw => stw.User)
            .FirstOrDefaultAsync();

        var investorIds = startup.StartupInvestors
            .Select(si => si.Investor.Id)
            .ToList();

        var scientistIds = startup.StartupScientists
            .Select(sc => sc.User.Id)
            .ToList();

        var workerIds = startup.StartupWorkers
            .Select(stw => stw.User.Id)
            .ToList();

        var allIds = investorIds.Concat(scientistIds).Concat(workerIds).ToList();

        foreach (var id in allIds)
        {
            await _notificationProducer.SendNotificationAsync(new NotificationDto
            {
                UserId = id,
                Body = "В чате стартапа",
                Title = "Новое сообщение!"
            });
        }


        await Clients.Group($"startup-{startupId}").SendAsync("ReceiveMessage", dto);
    }
}