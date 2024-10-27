using BookAPI.Dtos;

namespace BookAPI.Services.Interfaces
{
    public interface IChangeHistoryService
    {
        Task<IEnumerable<ChangeResponseDto>> GetAllChangeHistoriesAsync(ChangeHistoryQueryParams queryParams);
        Task<IEnumerable<ChangeResponseDto>> GetChangeHistoriesByIdAsync(Guid id);
    }
}
