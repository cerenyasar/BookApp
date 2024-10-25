using BookAPI.Repositories;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Database
{
    public interface IUnitOfWork
    {
        IBookRepository Books { get; }
        IAuthorRepository Authors { get; }
        IChangeHistoryRepository ChangeHistories { get; }

        Task CompleteAsync();
    }
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookDbContext _dbContext;
        public IBookRepository Books { get; private set; }
        public IAuthorRepository Authors { get; private set; }
        public IChangeHistoryRepository ChangeHistories { get; private set; }

        public UnitOfWork(BookDbContext dbContext)
        {
            _dbContext = dbContext;
            Books = new BookRepository(_dbContext);
            Authors = new AuthorRepository(_dbContext);           
        }

        public async Task CompleteAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

    }
}
