using BookAPI.Dtos.Validations;
using System.ComponentModel.DataAnnotations;

namespace BookAPI.Dtos
{
    public class AuthorDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [NotDefaultValue("string", ErrorMessage = "Name cannot be the default value.")]
        public string Name { get; set; }
    }
}
