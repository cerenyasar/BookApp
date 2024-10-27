using BookAPI.Entities;

namespace BookAPI.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetBookByIdAsync(Guid bookId);
        IQueryable<Book> GetAllBooks();

        void Add(Book book);
        void Update(Book book);
        void Delete(Book book);
    }
}
