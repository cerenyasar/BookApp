using BookAPI.Database;
using BookAPI.Entities;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookDbContext _dbContext;

        public BookRepository(BookDbContext context)
        {
            _dbContext = context;
        }

        public void Add(Book book) => _dbContext.Books.Add(book);

        public void Update(Book book) => _dbContext.Books.Update(book);

        public void Delete(Book book) => _dbContext.Books.Remove(book);


        public async Task<Book?> GetBookByIdAsync(Guid id)
        {
            return await _dbContext.Books.Include(b => b.BookAuthors)
                                   .ThenInclude(ba => ba.Author)
                                   .FirstOrDefaultAsync(b => b.Id == id);
        }
        public IQueryable<Book> GetAllBooks()
        {
            return _dbContext.Books.Include(b => b.BookAuthors)
                                   .ThenInclude(ba => ba.Author);
        }
    }
}
