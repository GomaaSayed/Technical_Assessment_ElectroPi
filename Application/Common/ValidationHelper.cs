using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public static class ValidationHelper
    {
        public static async Task<IActionResult> ValidateAndProcessAsync<T>(T request, IValidator<T> validator, Func<Task<IActionResult>> action)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult(validationResult.Errors);
            }
            return await action();
        }
    }

}
