using System;
using System.Collections.Generic;

namespace DormMVC.Model;

public partial class Room : Entity
{
    // Id успадковується з Entity, маппиться на ki_id через FluentAPI

    public int GuId { get; set; }
    public string? KiInformation { get; set; }
    public int? KiPoverh { get; set; }
    public int? KiMistkist { get; set; }
    public string KiNomer { get; set; } = null!;

    public virtual ICollection<Accommodation> Accommodations { get; set; } = new List<Accommodation>();
    public virtual Dorm Gu { get; set; } = null!;
}
