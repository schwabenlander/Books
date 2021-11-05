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

    public async Task<Book> GetBookAsync(Guid id) => 
        await _context.Books.Include(_ => _.Author).FirstOrDefaultAsync(_ => _.Id == id);

    public async Task<IEnumerable<Book>> GetBooksAsync() => 
        await _context.Books.Include(_ => _.Author).ToListAsync();
}
