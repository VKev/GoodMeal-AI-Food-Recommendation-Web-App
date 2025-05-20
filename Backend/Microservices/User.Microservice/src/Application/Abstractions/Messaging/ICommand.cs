using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.ResponseModel;
using MediatR;

namespace Application.Abstractions.Messaging
{
    public interface ICommand: IRequest<Result>
    {
    }
    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
    }
}