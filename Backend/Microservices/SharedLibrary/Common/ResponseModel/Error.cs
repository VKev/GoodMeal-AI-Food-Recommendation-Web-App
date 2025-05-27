using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.Common.ResponseModel
{
    public sealed record Error(string Code, string Description)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Error.NullValue", "Null value was provided");

        public static implicit operator Result(Error error) => Result.Failure(error);

        public Result ToResult() => Result.Failure(this);

        public static Error FromException(Exception ex)
        {
            var stackFrame = new StackFrame(1, false);
            var declaringType = stackFrame.GetMethod()?.DeclaringType;
            var errorSource = declaringType != null 
                ? $"{declaringType.Namespace}.{declaringType.Name}" 
                : "UnknownSource";

            return new Error(errorSource, ex.Message);
        }
    }
} 