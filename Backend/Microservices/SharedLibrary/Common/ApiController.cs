using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SharedLibrary.Common.ResponseModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SharedLibrary.Common
{
    public class ApiController : ControllerBase
    {
        protected readonly IMediator _mediator;
        protected ApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected IActionResult HandleFailure(Result result) =>
            result switch
            {
                { IsSuccess: true } => throw new InvalidOperationException(),
                IValidationResult validationResult =>
                    BadRequest(
                        CreateProblemDetails(
                            (int)HttpStatusCode.BadRequest,
                            result.Error,
                            validationResult.Errors
                        )
                    ),
                _ => 
                    BadRequest(
                        CreateProblemDetails(
                            (int)HttpStatusCode.BadRequest,
                            result.Error
                        )
                    ),
            };

        private static ProblemDetails CreateProblemDetails(int status, Error error, Error[]? errors = null) =>
            new()
            {
                Status = status,
                Type = error.Code,
                Detail = error.Description,
                Extensions = { { nameof(errors), errors } }
            };
    }
} 