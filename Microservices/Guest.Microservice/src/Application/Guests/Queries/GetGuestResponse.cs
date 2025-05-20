using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Guests.Queries
{
    public sealed record GetGuestResponse(
        string Fullname,
        string Email
    );
}