﻿namespace Books.Api.Models
{
    public class BookModel
    {
        public Guid Id { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
