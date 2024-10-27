namespace BookAPI.Dtos
{
    public class AuthorResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Books { get; set; }
    }
}
