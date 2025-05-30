using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Guestrole
{
    public int Roleid { get; set; }

    public string Rolename { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Guestrolemapping> Guestrolemappings { get; set; } = new List<Guestrolemapping>();
}
