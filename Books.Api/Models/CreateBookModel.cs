namespace Books.Api.Models;

public class CreateBookModel
{
    public Guid AuthorId { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
