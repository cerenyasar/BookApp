namespace BookAPI.Dtos
{
    public class BookCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }

        public List<string> Authors { get; set; }
    }
}
