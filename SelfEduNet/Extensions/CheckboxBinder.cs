using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SelfEduNet.Extensions
{
    public class CheckboxBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstOrDefault();

            if (value == "on")
            {
                bindingContext.Result = ModelBindingResult.Success(true);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(false);
            }

            return Task.CompletedTask;
        }
    }
}
