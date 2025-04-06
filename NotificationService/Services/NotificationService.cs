using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using NotificationService.DBModels;
using NotificationService.Models;
using Message = FirebaseAdmin.Messaging.Message;

namespace NotificationService.Services;

public interface INotificationService
{
    Task<bool> SendNotificationsAsync(MessageRequest request);
}

public class NotificationService(HackatonContext _context) : INotificationService
{
    public async Task<bool> SendNotificationsAsync(MessageRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if (user?.FcmToken == null)
            return false;
        
        var message = new Message()
        {
            Notification = new Notification
            {
                Title = request.Title,
                Body = request.Body
            },
            Token = user.FcmToken
        };
        var messaging = FirebaseMessaging.DefaultInstance;
        var result = await messaging.SendAsync(message);
        
        return !string.IsNullOrEmpty(result);
    }
}