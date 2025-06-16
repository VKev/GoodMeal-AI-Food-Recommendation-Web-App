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



        // New method for aggregating results with automatic numbering and visibility control
        public static Result<AggregatedResults> AggregateWithNumbers(params (Result result, bool show)[] results)
        {
            if (results == null || !results.Any())
                return Result.Success(new AggregatedResults { Results = new List<NamedResult>() });

            // Check for failures in all results (even those not shown)
            var firstFailure = results.FirstOrDefault(r => r.result.IsFailure);
            if (firstFailure.result != null)
            {
                // Preserve validation errors if the failed result is a ValidationResult
                if (firstFailure.result is IValidationResult validationResult)
                {
                    return ValidationResult<AggregatedResults>.WithErrors(validationResult.Errors);
                }
                return Result.Failure<AggregatedResults>(firstFailure.result.Error);
            }

            // All successful - create structured response with numbered names, only for results marked to show
            var namedResults = results
                .Select((item, index) => new { item, index })
                .Where(x => x.item.show)
                .Select(x => new NamedResult
                {
                    Name = (x.index + 1).ToString(),
                    IsSuccess = x.item.result.IsSuccess,
                    Data = GetResultData(x.item.result)
                }).ToList();

            return Result.Success(new AggregatedResults { Results = namedResults });
        }

        // Overload for backward compatibility - defaults all to not show (false)
        public static Result<AggregatedResults> AggregateWithNumbers(params Result[] results)
        {
            if (results == null || !results.Any())
                return Result.Success(new AggregatedResults { Results = new List<NamedResult>() });

            // Check for failures first
            var firstFailure = results.FirstOrDefault(r => r.IsFailure);
            if (firstFailure != null)
            {
                // Preserve validation errors if the failed result is a ValidationResult
                if (firstFailure is IValidationResult validationResult)
                {
                    return ValidationResult<AggregatedResults>.WithErrors(validationResult.Errors);
                }
                return Result.Failure<AggregatedResults>(firstFailure.Error);
            }

            // Convert to tuples with default show = false for backward compatibility
            var resultsWithVisibility = results.Select(r => (r, false)).ToArray();
            return AggregateWithNumbers(resultsWithVisibility);
        }



        private static object GetResultData(Result result)
        {
            if (result == null) return null;

            // Use reflection to get the Value property for Result<T>
            var resultType = result.GetType();
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueProperty = resultType.GetProperty("Value");
                if (valueProperty != null && result.IsSuccess)
                {
                    return valueProperty.GetValue(result);
                }
            }

            // For non-generic Result, return success status
            return new { IsSuccess = result.IsSuccess };
        }
    }

    // Supporting classes for structured response
    public class AggregatedResults
    {
        public List<NamedResult> Results { get; set; } = new();
    }

    public class NamedResult
    {
        public string Name { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public object? Data { get; set; }
    }
} 