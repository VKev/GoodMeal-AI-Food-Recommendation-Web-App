using System;
using System.Collections.Generic;
using System.Linq;
using SharedLibrary.Common.ResponseModel;

namespace SharedLibrary.Common
{
    public static class ResultAggregator
    {
        public static Result Aggregate(params Result[] results)
        {
            if (results == null || !results.Any())
                return Result.Success();

            var firstFailure = results.FirstOrDefault(r => r.IsFailure);
            if (firstFailure != null)
                return firstFailure;

            return Result.Success();
        }

        public static Result<T> Aggregate<T>(params Result<T>[] results)
        {
            if (results == null || !results.Any())
                return Result.Failure<T>(Error.None);

            var firstFailure = results.FirstOrDefault(r => r.IsFailure);
            if (firstFailure != null)
                return firstFailure;

            // If all are successful, return the last result
            return results.Last();
        }

        public static Result<IEnumerable<T>> AggregateAll<T>(params Result<T>[] results)
        {
            if (results == null || !results.Any())
                return Result.Success<IEnumerable<T>>(Enumerable.Empty<T>());

            var firstFailure = results.FirstOrDefault(r => r.IsFailure);
            if (firstFailure != null)
                return Result.Failure<IEnumerable<T>>(firstFailure.Error);

            // If all are successful, return all values
            return Result.Success<IEnumerable<T>>(results.Select(r => r.Value));
        }
    }
} 