using BookAPI.Dtos;
using BookAPI.Entities;

namespace BookAPI.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookResponseDto>> GetAllBooksAsync(BookQueryParameters bookQueryParameters);
        Task<BookResponseDto> GetBookByIdAsync(Guid id);
        Task<BookResponseDto> AddBookAsync(BookCreateDto bookCreateDto);
        Task<BookResponseDto> UpdateBookAsync(Guid id, BookUpdateDto bookUpdateDto);
        Task DeleteBookAsync(Guid id);
    }
}
