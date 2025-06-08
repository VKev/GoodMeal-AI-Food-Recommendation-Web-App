using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SharedLibrary.Common.ResponseModel
{

    public class Result
    {
        protected internal Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None ||
                !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        [JsonConstructor]
        protected Result()
        {
            IsSuccess = false;
            Error = Error.None;
        }

        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; protected set; }

        [JsonPropertyName("isFailure")]
        public bool IsFailure => !IsSuccess;

        [JsonPropertyName("error")]
        public Error Error { get; protected set; }

        public static Result Success() => new(true, Error.None);

        public static Result Failure(Error error) => new(false, error);

        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
    }

    public class Result<TValue> : Result
    {
        private TValue? _value;

        protected internal Result(TValue? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        [JsonConstructor]
        protected Result() : base()
        {
            _value = default;
        }

        [JsonPropertyName("value")]
        public TValue Value 
        { 
            get => IsSuccess
                ? _value!
                : throw new InvalidOperationException("The value of a failure result can't be accessed");
            protected set => _value = value;
        }

        public static implicit operator Result<TValue>(TValue? value) =>
            value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    }
}