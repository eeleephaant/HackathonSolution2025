using StartupService.Models;

namespace StartupService.Services;

public interface INotificationProducer
{
    public Task<bool> SendNotificationAsync(NotificationDto notification);
}

public class NotificationProducer(HttpClient httpClient) : INotificationProducer
{
    public async Task<bool> SendNotificationAsync(NotificationDto notification)
    {
        var response = await httpClient.PostAsJsonAsync("http://localhost:5444/notifications/send-notification", notification);
        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        return false;
    }
}