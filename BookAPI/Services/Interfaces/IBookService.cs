using BookAPI.Dtos;
using BookAPI.Entities;

namespace BookAPI.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book> AddBookAsync(BookCreateDto bookCreateDto);
    }
}
