using BookAPI.Dtos.Validations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookAPI.Dtos
{
    public class BookCreateDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [NotDefaultValue("string", ErrorMessage = "Title cannot be the default value.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [NotDefaultValue("string", ErrorMessage = "Description cannot be the default value.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Publish date is required.")]
        [NotDefaultDate(ErrorMessage = "Publish date cannot be the default date.")]
        public DateOnly PublishDate { get; set; }

        [Required(ErrorMessage = "At least one author is required.")]
        [MinLength(1, ErrorMessage = "Authors list cannot be empty.")]
        [NotPlaceholderString]
        public List<string> Authors { get; set; }
    }
}
