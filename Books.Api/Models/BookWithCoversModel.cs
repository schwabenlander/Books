namespace Books.Api.Models;

public class BookWithCoversModel : BookModel
{
    public IEnumerable<BookCoverModel> BookCovers { get; set; } = new List<BookCoverModel>();
}
