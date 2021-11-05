using Books.Api.Contexts;
using Books.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Books.Api.Services;

#nullable disable

public class BooksRepository : IBooksRepository
{
    private BooksContext _context;

    public BooksRepository(BooksContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void AddBook(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book));

        _context.Add(book);
    }

    public async Task<Book> GetBookAsync(Guid id) => 
        await _context.Books.Include(_ => _.Author).FirstOrDefaultAsync(_ => _.Id == id);

    public async Task<IEnumerable<Book>> GetBooksAsync() => 
        await _context.Books.Include(_ => _.Author).ToListAsync();

    public void AddBookCollection(IEnumerable<Book> books)
    {
        if (books == null)
            throw new ArgumentNullException(nameof(books));

        _context.AddRange(books);
    }

    public async Task<bool> SaveChangesAsync()
    {
        // returns true if 1 or more entities were changed
        return (await _context.SaveChangesAsync() > 0);
    }

    public async Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> bookIds)
    {
        return await _context.Books.Where(_ => bookIds.Contains(_.Id))
            .Include(_ => _.Author).ToListAsync();
    }
}
