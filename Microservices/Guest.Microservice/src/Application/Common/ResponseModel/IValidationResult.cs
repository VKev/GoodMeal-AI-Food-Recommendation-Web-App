using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Common.ResponseModel
{
    public interface IValidationResult
    {
        public static readonly Error ValidationError = new Error("Validation Error", "A Validation problem has occurred.");

        Error[] Errors { get; }

    }
}