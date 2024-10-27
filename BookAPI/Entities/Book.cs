namespace BookAPI.Entities
{
    public class Book
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly PublishDate { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; } // Many-to-many relationship
        public ICollection<ChangeHistory> ChangeHistories { get; set; } // One-to-many relationship
    }
}
