using System;
using System.Collections.Generic;

namespace DormDomain.Model;

public partial class Faculty : Entity
{

    public string? FaInformation { get; set; }
    public string? FaTelefon { get; set; }
    public string? FaKorpus { get; set; }
    public string? FaDekan { get; set; }
    public string FaName { get; set; } = null!;

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}