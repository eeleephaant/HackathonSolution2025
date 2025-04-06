using System;
using System.Collections.Generic;

namespace AIChatService.DBModels;

public partial class Skill
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UsersSkill> UsersSkills { get; set; } = new List<UsersSkill>();
}
