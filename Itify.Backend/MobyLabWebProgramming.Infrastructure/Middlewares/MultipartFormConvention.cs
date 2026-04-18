using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace MobyLabWebProgramming.Infrastructure.Middlewares;

public class MultipartFormConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var action in application.Controllers.SelectMany(e => e.Actions))
        {
            if (action.Parameters.Any(p => p.Attributes.OfType<FromFormAttribute>().Any()) && 
                !action.Filters.OfType<ConsumesAttribute>().Any())
            {
                action.Filters.Add(new ConsumesAttribute("multipart/form-data"));
            }
        }
    }
}