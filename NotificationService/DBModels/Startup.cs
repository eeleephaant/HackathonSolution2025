using System;
using System.Collections.Generic;

namespace NotificationService.DBModels;

public partial class Startup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? OwnerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User? Owner { get; set; }

    public virtual ICollection<StartupInvestor> StartupInvestors { get; set; } = new List<StartupInvestor>();

    public virtual ICollection<StartupScientist> StartupScientists { get; set; } = new List<StartupScientist>();

    public virtual ICollection<StartupWorker> StartupWorkers { get; set; } = new List<StartupWorker>();
}
