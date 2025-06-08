using System;
using System.Collections.Generic;
using System.Linq;
using SharedLibrary.Common.ResponseModel;

namespace SharedLibrary.Common
{
    public static class ResultAggregator
    {
        /// <summary>
        /// Aggregates multiple results. Returns success if all results are successful, otherwise returns the first failure.
        /// </summary>
        /// <param name="results">Collection of results to aggregate</param>
        /// <returns>Success result if all are successful, otherwise the first failure</returns>
        public static Result Aggregate(params Result[] results)
        {
            if (results == null || !results.Any())
                return Result.Failure(new Error("ResultAggregator.NoResults", "No results provided for aggregation"));

            var firstFailure = results.FirstOrDefault(r => r.IsFailure);
            return firstFailure ?? Result.Success();
        }

        /// <summary>
        /// Aggregates multiple results. Returns success if all results are successful, otherwise returns the first failure.
        /// </summary>
        /// <param name="results">Collection of results to aggregate</param>
        /// <returns>Success result if all are successful, otherwise the first failure</returns>
        public static Result Aggregate(IEnumerable<Result> results)
        {
            return Aggregate(results.ToArray());
        }

        /// <summary>
        /// Aggregates multiple results with values. Returns aggregated values if all are successful, otherwise returns the first failure.
        /// </summary>
        /// <typeparam name="T">Type of the result values</typeparam>
        /// <param name="results">Collection of results with values to aggregate</param>
        /// <returns>Success result with aggregated values if all are successful, otherwise the first failure</returns>
        public static Result<IEnumerable<T>> AggregateValues<T>(params Result<T>[] results)
        {
            if (results == null || !results.Any())
                return Result.Failure<IEnumerable<T>>(new Error("ResultAggregator.NoResults", "No results provided for aggregation"));

            var firstFailure = results.FirstOrDefault(r => r.IsFailure);
            if (firstFailure != null)
                return Result.Failure<IEnumerable<T>>(firstFailure.Error);

            var values = results.Select(r => r.Value);
            return Result.Success(values);
        }

        /// <summary>
        /// Aggregates multiple results with values. Returns aggregated values if all are successful, otherwise returns the first failure.
        /// </summary>
        /// <typeparam name="T">Type of the result values</typeparam>
        /// <param name="results">Collection of results with values to aggregate</param>
        /// <returns>Success result with aggregated values if all are successful, otherwise the first failure</returns>
        public static Result<IEnumerable<T>> AggregateValues<T>(IEnumerable<Result<T>> results)
        {
            return AggregateValues(results.ToArray());
        }

        /// <summary>
        /// Aggregates multiple results with different value types into a list of objects.
        /// Returns aggregated values if all are successful, otherwise returns the first failure.
        /// </summary>
        /// <param name="results">Collection of results with different value types to aggregate</param>
        /// <returns>Success result with aggregated values as objects if all are successful, otherwise the first failure</returns>
        public static Result<IEnumerable<object>> AggregateValues(params object[] results)
        {
            if (results == null || !results.Any())
                return Result.Failure<IEnumerable<object>>(new Error("ResultAggregator.NoResults", "No results provided for aggregation"));

            var resultObjects = new List<object>();
            
            foreach (var result in results)
            {
                if (result is Result baseResult)
                {
                    if (baseResult.IsFailure)
                        return Result.Failure<IEnumerable<object>>(baseResult.Error);
                    
                    // For Result<T>, extract the value
                    if (result.GetType().IsGenericType && 
                        result.GetType().GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        var valueProperty = result.GetType().GetProperty("Value");
                        if (valueProperty != null)
                        {
                            resultObjects.Add(valueProperty.GetValue(result)!);
                        }
                    }
                }
                else
                {
                    resultObjects.Add(result);
                }
            }

            return Result.Success(resultObjects.AsEnumerable());
        }
    }
} 