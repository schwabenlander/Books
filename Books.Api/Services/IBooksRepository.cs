using Books.Api.Entities;

namespace Books.Api.Services;
public interface IBooksRepository
{
    Task<IEnumerable<Book>> GetBooksAsync();

    Task<Book> GetBookAsync(Guid id);
}
