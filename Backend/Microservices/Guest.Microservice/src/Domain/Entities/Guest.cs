using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Guest
{
    public int Guestid { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phonenumber { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual ICollection<Guestrolemapping> Guestrolemappings { get; set; } = new List<Guestrolemapping>();
}
