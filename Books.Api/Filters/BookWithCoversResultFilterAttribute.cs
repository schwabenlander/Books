using AutoMapper;
using Books.Api.Entities;
using Books.Api.ExternalModels;
using Books.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Books.Api.Filters;

public class BookWithCoversResultFilterAttribute : ResultFilterAttribute
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

        (var book, var bookCovers) = ((Book, IEnumerable<BookCover>))result.Value;

        var mapper = context.HttpContext.RequestServices.GetRequiredService<IMapper>();

        var mappedBook = mapper.Map<BookWithCoversModel>(book);
        result.Value = mapper.Map(bookCovers, mappedBook);

        await next();
    }
}
