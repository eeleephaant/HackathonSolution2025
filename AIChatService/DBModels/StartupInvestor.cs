using System;
using System.Collections.Generic;

namespace AIChatService.DBModels;

public partial class StartupInvestor
{
    public int Id { get; set; }

    public int? StartupId { get; set; }

    public int? InvestorId { get; set; }

    public virtual User? Investor { get; set; }

    public virtual Startup? Startup { get; set; }
}
