using BookAPI.Dtos;

namespace BookAPI.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<AuthorResponseDto> AddAuthorAsync(AuthorDto authorCreateDto);
        Task<IEnumerable<AuthorResponseDto>> GetAllAuthorsAsync();
        Task<AuthorResponseDto> GetAuthorByIdAsync(Guid id);
        Task<AuthorResponseDto> GetAuthorByNameAsync(string name);
        Task<AuthorResponseDto> UpdateAuthorAsync(Guid id, AuthorDto authorDto);
        Task DeleteAuthorAsync(Guid id);
    }
}
