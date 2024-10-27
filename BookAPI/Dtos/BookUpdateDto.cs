﻿namespace BookAPI.Dtos
{
    public class BookUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly? PublishDate { get; set; }

        public List<string> Authors { get; set; }
    }
}
