namespace BookAPI.Dtos
{
    public class AuthorUpdateDto
    {
        public string Name { get; set; }
        public List<string> Books { get; set; }
    }
}
