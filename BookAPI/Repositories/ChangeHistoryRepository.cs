using BookAPI.Database;
using BookAPI.Entities;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace BookAPI.Repositories
{
    public class ChangeHistoryRepository : IChangeHistoryRepository
    {
        private readonly BookDbContext _dbContext;

        public ChangeHistoryRepository(BookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(ChangeHistory changeHistory) => _dbContext.ChangeHistories.Add(changeHistory);

        public void Delete(ChangeHistory changeHistory) => _dbContext.ChangeHistories.Remove(changeHistory);

        public void Update(ChangeHistory changeHistory) => _dbContext.ChangeHistories.Update(changeHistory);

        public async Task<IEnumerable<ChangeHistory>> GetHistoriesByBookIdAsync(Guid bookId)
        {
            return await _dbContext.ChangeHistories
                .Where(ch => ch.BookId == bookId)
                .Include(ch => ch.Book)
                .ToListAsync();
        }

        public IQueryable<ChangeHistory> GetAllHistories()
        {
            return _dbContext.ChangeHistories.Include(ch => ch.Book);
        }
    }
}
