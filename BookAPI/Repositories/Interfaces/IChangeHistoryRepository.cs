using BookAPI.Entities;

namespace BookAPI.Repositories.Interfaces
{
    public interface IChangeHistoryRepository
    {
        Task<IEnumerable<ChangeHistory>> GetHistoriesByBookIdAsync(Guid bookId);
        IQueryable<ChangeHistory> GetAllHistories();

        void Add(ChangeHistory changeHistory);
        void Update(ChangeHistory changeHistory);
        void Delete(ChangeHistory changeHistory);
    }
}
