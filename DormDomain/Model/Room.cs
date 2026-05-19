using System;
using System.Collections.Generic;

namespace DormDomain.Model;

public partial class Room : Entity
{

    public int GuId { get; set; }
    public string? KiInformation { get; set; }
    public int? KiPoverh { get; set; }
    public int? KiMistkist { get; set; }
    public string KiNomer { get; set; } = null!;

    public virtual ICollection<Accommodation> Accommodations { get; set; } = new List<Accommodation>();
    public virtual Dorm Gu { get; set; } = null!;
}
