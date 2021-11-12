using Books.Api.Contexts;
using Books.Api.Entities;
using Books.Api.ExternalModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Books.Api.Services;

#nullable disable

public class BooksRepository : IBooksRepository, IDisposable
{
    private readonly BooksContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BooksRepository> _logger;
    private CancellationTokenSource _cancellationTokenSource;

    public BooksRepository(BooksContext context, 
        IHttpClientFactory httpClientFactory, 
        ILogger<BooksRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _httpClientFactory = httpClientFactory;
        _logger = logger;
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
        string bookCoverUrl, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(bookCoverUrl, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var bookCover = JsonSerializer.Deserialize<BookCover>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return bookCover;
        }
        else
        {
            _cancellationTokenSource.Cancel();
            return null;
        }
    }

    public async Task<IEnumerable<BookCover>> GetBookCoversAsync(Guid bookId)
    {
        _cancellationTokenSource = new CancellationTokenSource();
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

        var downloadBookCoverTasks = bookCoverUrls
            .Select(url => DownloadBookCoverAsync(httpClient, url, _cancellationTokenSource.Token))
            .ToList();

        try
        {
            return await Task.WhenAll(downloadBookCoverTasks);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError($"{ex.Message}");

            foreach (var task in downloadBookCoverTasks)
            {
                _logger.LogInformation($"Task {task.Id} has the status {task.Status}");
            }

            return new List<BookCover>();
        }
    }

    public void Dispose()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}
