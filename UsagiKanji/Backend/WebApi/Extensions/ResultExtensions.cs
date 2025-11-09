using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(
            this Result<T> result,
            ControllerBase controller,
            Func<T, IActionResult>? onSuccess = null)
        {
            if (result.IsSuccess)
            {
                return onSuccess != null ? onSuccess(result.Value) : controller.Ok(result.Value);
            }

            var firstError = result.Errors.FirstOrDefault()?.Message ?? "Unknown error";

            int statusCode = result.Errors.FirstOrDefault()?.Metadata.ContainsKey("StatusCode") == true
                ? (int)result.Errors.First().Metadata["StatusCode"]!
                : 400;

            var problem = new ProblemDetails
            {
                Title = "Request failed",
                Detail = firstError,
                Status = statusCode
            };

            return controller.Problem(problem.Detail, null, problem.Status);
        }
    }
}
