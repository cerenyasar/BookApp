using BookAPI.Entities;

namespace BookAPI.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetBookByIdAsync(Guid bookId);
        Task<IEnumerable<Book>> GetAllBooksAsync();

        void Add(Book book);
        void Update(Book book);
        void Delete(Book book);
    }
}
