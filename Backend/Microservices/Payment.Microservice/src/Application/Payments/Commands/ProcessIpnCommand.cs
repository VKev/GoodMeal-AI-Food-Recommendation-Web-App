using Microsoft.AspNetCore.Http;
using MediatR;
using FluentValidation;
using SharedLibrary.Common.ResponseModel;

namespace Application.Payments.Commands;

public record ProcessIpnCommand(IQueryCollection QueryParameters) : IRequest<Result<bool>>;

public class ProcessIpnCommandValidator : AbstractValidator<ProcessIpnCommand>
{
    public ProcessIpnCommandValidator()
    {
        RuleFor(x => x.QueryParameters).NotEmpty();
    }
} 