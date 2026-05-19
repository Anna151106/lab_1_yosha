using System;
using System.Collections.Generic;

namespace DormMVC.Model;

public partial class Department : Entity
{
    // Id успадковується з Entity, маппиться на ka_id через FluentAPI

    public int FaId { get; set; }
    public string? KaInformation { get; set; }
    public string? KaTelefon { get; set; }
    public string? KaZaviduvach { get; set; }
    public string KaName { get; set; } = null!;

    public virtual Faculty Fa { get; set; } = null!;
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
