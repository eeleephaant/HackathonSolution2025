using System;
using System.Collections.Generic;

namespace UserService.DBModels;

public partial class Message
{
    public int Id { get; set; }

    public int StartupId { get; set; }

    public int SenderId { get; set; }

    public string? Text { get; set; }

    public string? AttachmentUrl { get; set; }

    public string? AttachmentType { get; set; }

    public DateTime SentAt { get; set; }

    public virtual User Sender { get; set; } = null!;

    public virtual Startup Startup { get; set; } = null!;
}
