namespace NotificationService.Models;

public class MessageRequest
{
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
}