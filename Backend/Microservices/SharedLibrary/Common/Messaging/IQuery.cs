using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary.Common.ResponseModel;
using MediatR;

namespace SharedLibrary.Common.Messaging
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }

}