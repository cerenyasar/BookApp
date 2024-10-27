using BookAPI.Database;
using BookAPI.Entities;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class BookAuthorRepository : IBookAuthorRepository
    {
        private readonly BookDbContext _dbContext;

        public BookAuthorRepository(BookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddBookAuthorAsync(Book book, Author author)
        {
            var bookAuthor = new BookAuthor { Book = book, Author = author };

            // Check if the relationship already exists
            if (!_dbContext.BookAuthors.Any(ba => ba.BookId == book.Id && ba.AuthorId == author.Id))
            {
                await _dbContext.BookAuthors.AddAsync(bookAuthor);
            }
        }

        public void RemoveBookAuthor(BookAuthor bookAuthor)
        {            
            _dbContext.BookAuthors.Remove(bookAuthor);            
        }

        //public async Task RemoveBookAuthorAsync(Book book, Author author)
        //{
        //    var bookAuthor = await _dbContext.BookAuthors
        //     .FirstOrDefaultAsync(ba => ba.BookId == book.Id && ba.AuthorId == author.Id);

        //    if (bookAuthor != null)
        //    {
        //        _dbContext.BookAuthors.Remove(bookAuthor);
        //    }
        //}
    }
}
