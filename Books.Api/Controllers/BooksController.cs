using AutoMapper;
using Books.Api.Entities;
using Books.Api.Filters;
using Books.Api.Models;
using Books.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : Controller
{
    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;

    public BooksController(IBooksRepository booksRepository, IMapper mapper)
    {
        _booksRepository = booksRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [BooksResultFilter]
    public async Task<IActionResult> GetBooksAsync() => Ok(await _booksRepository.GetBooksAsync());

    [HttpGet]
    [Route("{id}", Name = "GetBook")]
    [BookResultFilter]
    public async Task<IActionResult> GetBookAsync(Guid id)
    {
        Book book = await _booksRepository.GetBookAsync(id);

        if (book is null) return NotFound();

        var bookCovers = await _booksRepository.GetBookCoversAsync(id);

        return Ok(book);
    }

    [HttpPost]
    [BookResultFilter]
    public async Task<IActionResult> CreateBook(CreateBookModel newBook)
    {
        var bookEntity = _mapper.Map<Book>(newBook);

        _booksRepository.AddBook(bookEntity);

        await _booksRepository.SaveChangesAsync();

        await _booksRepository.GetBookAsync(bookEntity.Id);

        return CreatedAtRoute("GetBook", new { id = bookEntity.Id }, bookEntity);
    }
}
