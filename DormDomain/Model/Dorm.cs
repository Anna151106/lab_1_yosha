using System;
using System.Collections.Generic;

namespace DormDomain.Model;

public partial class Dorm : Entity
{

    public string GuNomer { get; set; } = null!;
    public string? GuAdresa { get; set; }
    public int? GuKilkistPoverh { get; set; }
    public string? GuKomendant { get; set; }
    public string? GuInformation { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
