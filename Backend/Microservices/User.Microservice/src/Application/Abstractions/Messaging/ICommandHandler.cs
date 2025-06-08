using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using SharedLibrary.Common.ResponseModel;
using MediatR;

namespace Application.Abstractions.Messaging
{
    public interface ICommandHandler<TCommand>: IRequestHandler<TCommand, Result> where TCommand : ICommand 
    {
    }

    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>> where TCommand : ICommand<TResponse>
    {
    }
}