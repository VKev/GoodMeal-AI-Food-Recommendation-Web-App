using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Users.Commands
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().MaximumLength(70).EmailAddress();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(70);
        }
    }
}