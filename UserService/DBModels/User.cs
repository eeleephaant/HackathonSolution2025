using System;
using System.Collections.Generic;

namespace UserService.DBModels;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string ThirdName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? Role { get; set; }

    public string? FcmToken { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Role? RoleNavigation { get; set; }

    public virtual ICollection<StartupInvestor> StartupInvestors { get; set; } = new List<StartupInvestor>();

    public virtual ICollection<StartupScientist> StartupScientists { get; set; } = new List<StartupScientist>();

    public virtual ICollection<StartupWorker> StartupWorkers { get; set; } = new List<StartupWorker>();

    public virtual ICollection<Startup> Startups { get; set; } = new List<Startup>();

    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

    public virtual ICollection<UsersSkill> UsersSkills { get; set; } = new List<UsersSkill>();

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();
}
