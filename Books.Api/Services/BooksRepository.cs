using Books.Api.Contexts;
using Books.Api.Entities;
using Books.Api.ExternalModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Books.Api.Services;

#nullable disable

public class BooksRepository : IBooksRepository
{
    private BooksContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public BooksRepository(BooksContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        this._httpClientFactory = httpClientFactory;
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

    public async Task<BookCover> GetBookCoverAsync(string coverId)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync($"https://localhost:7140/api/bookcovers/{coverId}");

        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<BookCover>(
                await response.Content.ReadAsStringAsync(), 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        return null;
    }

    public async Task<BookCover> DownloadBookCoverAsync(HttpClient httpClient,
        string bookCoverUrl)
    {
        return await httpClient.GetFromJsonAsync<BookCover>(bookCoverUrl,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<IEnumerable<BookCover>> GetBookCoversAsync(Guid bookId)
    {
        var httpClient = _httpClientFactory.CreateClient();
        List<BookCover> bookCovers = new();

        // create a list of fake book covers
        var bookCoverUrls = new[]
        {
            $"https://localhost:7140/api/bookcovers/{bookId}-dummycover1",
            $"https://localhost:7140/api/bookcovers/{bookId}-dummycover2",
            $"https://localhost:7140/api/bookcovers/{bookId}-dummycover3",
            $"https://localhost:7140/api/bookcovers/{bookId}-dummycover4",
            $"https://localhost:7140/api/bookcovers/{bookId}-dummycover5"
        };

        var downloadBookCoverTasks = bookCoverUrls.Select(url => DownloadBookCoverAsync(httpClient, url)).ToList();

        return await Task.WhenAll(downloadBookCoverTasks);
    }
}
