using System;
using System.Collections.Generic;

namespace NotificationService.DBModels;

public partial class UsersSkill
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? SkillId { get; set; }

    public virtual Skill? Skill { get; set; }

    public virtual User? User { get; set; }
}
