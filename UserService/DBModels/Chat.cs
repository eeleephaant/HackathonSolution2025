using System;
using System.Collections.Generic;

namespace UserService.DBModels;

public partial class Chat
{
    public int Id { get; set; }

    public int? StartupId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Startup? Startup { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
