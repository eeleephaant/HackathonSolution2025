using System;
using System.Collections.Generic;

namespace UserService.DBModels;

public partial class Investor
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
