using System;
using System.Collections.Generic;

namespace NotificationService.DBModels;

public partial class UserSession
{
    public Guid Id { get; set; }

    public int? UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastAccessedAt { get; set; }

    public string LastAccessedIp { get; set; } = null!;

    public string LastAccessedUserAgent { get; set; } = null!;

    public string Token { get; set; } = null!;

    public virtual User? User { get; set; }
}
