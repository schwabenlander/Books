using Books.Api.Entities;

namespace Books.Api.Services;
public interface IBooksRepository
{
    Task<IEnumerable<Book>> GetBooksAsync();

    Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> bookIds);

    Task<Book> GetBookAsync(Guid id);

    void AddBook(Book book);

    void AddBookCollection(IEnumerable<Book> books);

    Task<bool> SaveChangesAsync();
}
