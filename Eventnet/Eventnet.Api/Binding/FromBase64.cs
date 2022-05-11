using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Eventnet.Api.Binding;

public class Base64ModelBinderProvider<TModel> : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return context.Metadata.ModelType == typeof(TModel) ? new FromBase64Binder() : null;
    }
}

public class FromBase64Binder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value))
            return Task.CompletedTask;

        try
        {
            var bytes = Convert.FromBase64String(value);
            var json = Encoding.UTF8.GetString(bytes);
            bindingContext.Result = ModelBindingResult.Success(JsonSerializer.Deserialize(json, bindingContext.ModelType));
        }
        catch (Exception)
        {
            bindingContext.Model = null;
        }

        return Task.CompletedTask;
    }
}