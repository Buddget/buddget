using System;

namespace Buddget.BLL.Utilities
{
    public class Result<T>
    {
        private Result(T value, bool success, string errorMessage = "Unexpected exception occured!")
        {
            Value = value;
            Success = success;
            ErrorMessage = errorMessage;
        }

        public T Value { get; private set; }
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }

        public static Result<T> SuccessResult(T value) => new Result<T>(value, true);

        public static Result<T> FailureResult(string errorMessage)
        {
            return new Result<T>(default, false, errorMessage);
        }
    }
}
