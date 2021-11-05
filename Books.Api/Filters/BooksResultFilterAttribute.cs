using AutoMapper;
using Books.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Books.Api.Filters;

public class BooksResultFilterAttribute : ResultFilterAttribute
{
    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var result = context.Result as ObjectResult;

        if (result?.Value == null ||
            result.StatusCode < 200 ||
            result.StatusCode >= 300)
        {
            await next();
            return;
        }

        var mapper = context.HttpContext.RequestServices.GetRequiredService<IMapper>();

        result.Value = mapper.Map<IEnumerable<BookModel>>(result.Value);

        await next();
    }
}
