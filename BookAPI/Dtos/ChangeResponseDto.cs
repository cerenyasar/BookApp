namespace BookAPI.Dtos
{
    public class ChangeResponseDto
    {
        public Guid Id { get; set; } 
        public Guid BookId { get; set; }
        public string BookTitle { get; set; }
        public DateTime ChangeTime { get; set; }
        public string ChangeDescription { get; set; }
    }
}
