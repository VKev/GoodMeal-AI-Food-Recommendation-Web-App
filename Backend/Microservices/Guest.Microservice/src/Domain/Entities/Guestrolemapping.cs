using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Guestrolemapping
{
    public int Guestid { get; set; }

    public int Roleid { get; set; }

    public DateTime? Assignedat { get; set; }

    public virtual Guest Guest { get; set; } = null!;

    public virtual Guestrole Role { get; set; } = null!;
}
