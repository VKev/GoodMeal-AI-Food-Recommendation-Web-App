using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Application.Guests.Commands;
using FluentValidation;

namespace Application.Guests.Commands
{
    public class CreateGuestValidator : AbstractValidator<CreateGuestCommand>
    {
        public CreateGuestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().MaximumLength(70).EmailAddress();
            RuleFor(x => x.Fullname).NotEmpty().MaximumLength(70);
        }
    }
}