using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Users.Queries
{
    public sealed record GetUserResponse(
        string Name,
        string Email
    );
}