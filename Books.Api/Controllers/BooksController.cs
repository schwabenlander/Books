using Books.Api.Entities;
using Books.Api.Filters;
using Books.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : Controller
{
    private readonly IBooksRepository _booksRepository;

    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }

    [HttpGet]
    [BooksResultFilter]
    public async Task<IActionResult> GetBooksAsync() => Ok(await _booksRepository.GetBooksAsync());

    [HttpGet]
    [Route("{id}")]
    [BookResultFilter]
    public async Task<IActionResult> GetBookByIdAsync(Guid id)
    {
        Book book = await _booksRepository.GetBookAsync(id);

        if (book is null) return NotFound();

        return Ok(book);
    }
}
