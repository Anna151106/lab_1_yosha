using System;
using System.Collections.Generic;

namespace DormMVC.Model;

public partial class Dorm : Entity
{
    // Id успадковується з Entity, маппиться на gu_id через FluentAPI

    public string GuNomer { get; set; } = null!;
    public string? GuAdresa { get; set; }
    public int? GuKilkistPoverh { get; set; }
    public string? GuKomendant { get; set; }
    public string? GuInformation { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
