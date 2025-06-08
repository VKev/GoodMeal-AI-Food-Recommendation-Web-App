using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.Common.ResponseModel
{
    public sealed class ValidationResult : Result, IValidationResult
    {
        public ValidationResult(Error[] errors) : base(false, IValidationResult.ValidationError) => Errors = errors;

        public Error[] Errors { get; }

        public static ValidationResult WithErrors(Error[] errors) => new(errors);

    }

    public class ValidationResult<TValue> : Result<TValue>, IValidationResult
    {
        public ValidationResult(Error[] errors) : base(default, false, IValidationResult.ValidationError) => Errors = errors;

        public Error[] Errors { get; }

        public static ValidationResult<TValue> WithErrors(Error[] errors) => new(errors);

    }
} 