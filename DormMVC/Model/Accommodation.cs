using System;
using System.Collections.Generic;

namespace DormMVC.Model;

public partial class Accommodation : Entity
{
    // Id успадковується з Entity, маппиться на pr_id через FluentAPI

    public int KiId { get; set; }
    public int StId { get; set; }
    public DateOnly? PrDataVysel { get; set; }
    public DateOnly PrDataZasel { get; set; }

    public virtual Room Ki { get; set; } = null!;
    public virtual Student St { get; set; } = null!;
}
