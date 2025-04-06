using ChatService.Models;

namespace ChatService.Models;

public class MessageDTO
{
    public int Id { get; set; }

    public int StartupId { get; set; }              // ID стартапа (комнаты)
    public int SenderId { get; set; }               // ID отправителя
    public UserDTO Sender { get; set; } = null!;    // Краткая инфа об отправителе

    public string? Text { get; set; }               // Текст сообщения (может быть null)
	
    public string? AttachmentUrl { get; set; }      // Ссылка на файл (если есть)
    public string? AttachmentType { get; set; }     // MIME-тип: "audio/webm", "image/png", "application/pdf" и т.п.

    public DateTime SentAt { get; set; }            // Время отправки
}