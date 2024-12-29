using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Models;

namespace SelfEduNet.Areas.Admin.Helpers
{
    public class OwnerOnlyAttribute : ActionFilterAttribute
    {
        private readonly string _courseIdParamName;

        public OwnerOnlyAttribute(string courseIdParamName)
        {
            _courseIdParamName = courseIdParamName;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var courseId = context.ActionArguments[_courseIdParamName]?.ToString();
            if (string.IsNullOrEmpty(courseId))
            {
                context.Result = new BadRequestObjectResult("Course ID is required.");
                return;
            }

            var userId = context.HttpContext.User.Identity.Name; // Получаем Id текущего пользователя

            // Проверка, что текущий пользователь является владельцем курса
            //var course = await GetCourseByIdAsync(courseId); // Реализуйте метод для получения курса по Id
            //if (course?.OwnerId != userId)
            //{
            //    context.Result = new ForbidResult(); // Запрещаем доступ, если пользователь не владелец
            //    return;
            //}

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
