using System;
using System.Collections.Generic;

namespace NotificationService.DBModels;

public partial class StartupWorker
{
    public int Id { get; set; }

    public int? StartupId { get; set; }

    public int? UserId { get; set; }

    public virtual Startup? Startup { get; set; }

    public virtual User? User { get; set; }
}
