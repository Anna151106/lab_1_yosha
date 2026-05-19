using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DormDomain.Model;

public partial class Accommodation : Entity
{

    public int KiId { get; set; }
    public int StId { get; set; }
    public DateOnly? PrDataVysel { get; set; }
    public DateOnly PrDataZasel { get; set; }

    public virtual Room Ki { get; set; } = null!;
    public virtual Student St { get; set; } = null!;
}