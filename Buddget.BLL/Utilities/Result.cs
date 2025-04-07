using System;

namespace Buddget.BLL.Utilities
{
    public class Result<T>
    {
        private Result(T value, bool success, string errorMessage)
        {
            Value = value;
            Success = success;
            ErrorMessage = errorMessage ?? "Unexpected exception occurred!";
        }

        public T Value { get; private set; }
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }

        public static Result<T> SuccessResult(T value) => new Result<T>(value, true, string.Empty);

        public static Result<T> FailureResult(string errorMessage)
        {
            return new Result<T>(default!, false, errorMessage);
        }
    }
}
