namespace BookAPI.Entities
{
    public class Author
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; } // Many-to-many with books
    }
}
