namespace BookAPI.Entities
{
    public class ChangeHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        public DateTime ChangeTime { get; set; }
        public string ChangeDescription { get; set; }
    }
}
