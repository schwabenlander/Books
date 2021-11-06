using Books.Api.Entities;
using Books.Api.ExternalModels;

namespace Books.Api.Services;

public interface IBooksRepository
{
    Task<IEnumerable<Book>> GetBooksAsync();

    Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> bookIds);

    Task<Book> GetBookAsync(Guid id);

    Task<BookCover> GetBookCoverAsync(string coverId);

    Task<IEnumerable<BookCover>> GetBookCoversAsync(Guid bookId);

    void AddBook(Book book);

    void AddBookCollection(IEnumerable<Book> books);

    Task<bool> SaveChangesAsync();
}
