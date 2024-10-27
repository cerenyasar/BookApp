namespace BookAPI.Dtos
{
    public class BookQueryParameters
    {
        //Default values
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;

        public string? Filter { get; set; } //Optional
        public string? Sort { get; set; } //Optional

    }
}
