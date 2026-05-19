using System;
using System.Collections.Generic;

namespace DormMVC.Model;

public partial class Student : Entity
{
    // Id успадковується з Entity, маппиться на st_id через FluentAPI

    public int FaId { get; set; }
    public int KaId { get; set; }
    public string StPib { get; set; } = null!;
    public int? StKurs { get; set; }
    public string? StTelefon { get; set; }
    public DateOnly? StDataNarodz { get; set; }

    public virtual ICollection<Accommodation> Accommodations { get; set; } = new List<Accommodation>();
    public virtual Faculty Fa { get; set; } = null!;
    public virtual Department Ka { get; set; } = null!;
}