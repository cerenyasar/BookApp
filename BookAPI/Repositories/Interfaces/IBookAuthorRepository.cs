using BookAPI.Entities;
using System.Threading.Tasks;

namespace BookAPI.Repositories.Interfaces
{
    public interface IBookAuthorRepository
    {
        Task AddBookAuthorAsync(Book book, Author author);
        void RemoveBookAuthor(BookAuthor bookAuthor);
    }
}
