using AutoMapper;
using Books.Api.Entities;
using Books.Api.Filters;
using Books.Api.ModelBinders;
using Books.Api.Models;
using Books.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers
{
    [ApiController]
    [Route("api/bookcollections")]
    [BooksResultFilter]
    public class BookCollectionsController : Controller
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public BookCollectionsController(IBooksRepository booksRepository, IMapper mapper)
        {
            _booksRepository = booksRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> CreateBookCollection(IEnumerable<CreateBookModel> bookCollection)
        {
            var bookEntities = _mapper.Map<IEnumerable<Book>>(bookCollection);

            _booksRepository.AddBookCollection(bookEntities);

            await _booksRepository.SaveChangesAsync();

            var booksToReturn = await _booksRepository.GetBooksAsync(bookEntities.Select(_ => _.Id));

            var bookIds = string.Join(",", booksToReturn.Select(_ => _.Id));

            return CreatedAtRoute("GetBookCollection", new { bookIds }, booksToReturn);
        }

        [HttpGet("({bookIds})", Name = "GetBookCollection")]
        public async Task<IActionResult> GetBookCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> bookIds)
        {
            var bookEntities = await _booksRepository.GetBooksAsync(bookIds);

            if (bookIds.Count() != bookEntities.Count())
                return NotFound();

            return Ok(bookEntities);
        }
    }
}
