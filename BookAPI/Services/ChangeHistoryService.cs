using BookAPI.Database;
using BookAPI.Dtos;
using BookAPI.Entities;
using BookAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookAPI.Services
{
    public class ChangeHistoryService : IChangeHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChangeHistoryService> _logger;

        public ChangeHistoryService(IUnitOfWork unitOfWork, ILogger<ChangeHistoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<ChangeResponseDto>> GetAllChangeHistoriesAsync(ChangeHistoryQueryParams queryParams)
        {
            IQueryable<ChangeHistory> query = _unitOfWork.ChangeHistories.GetAllHistories();

            try
            {
                // Apply Filtering
                if (!string.IsNullOrEmpty(queryParams.Filter))
                {
                    // Implement parsing of filter (e.g., split on ":" and match properties)
                    if (queryParams.Filter.StartsWith("title:"))
                    {
                        var authorName = queryParams.Filter.Split(":")[1];
                        query = query.Where(h => h.Book.BookAuthors.Any(ba => ba.Book.Title.Contains(authorName)));
                    }

                    if (queryParams.Filter.StartsWith("author:"))
                    {
                        var authorName = queryParams.Filter.Split(":")[1];
                        query = query.Where(h => h.Book.BookAuthors.Any(ba => ba.Author.Name.Contains(authorName)));
                    }
                }

                // Apply Sorting
                query = queryParams.Sort switch
                {
                    "title" => query.OrderBy(h => h.Book.Title),
                    "date" => query.OrderBy(h => h.ChangeTime),
                    _ => query
                };

                // Apply Grouping
                if (!string.IsNullOrEmpty(queryParams.GroupBy))
                {
                    if (queryParams.GroupBy == "author")
                    {
                        query.GroupBy(h => h.Book.BookAuthors.Select(ba => ba.Author.Name));
                    }
                    else if (queryParams.GroupBy == "title")
                    {
                        query.GroupBy(h => h.Book.Title);
                    }
                }

                // Pagination
                query = query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);

                var historyDtos = await query.Select(h => new ChangeResponseDto
                {
                    Id = h.Id,
                    BookId = h.BookId,
                    BookTitle = h.Book.Title,
                    ChangeDescription = h.ChangeDescription,
                    ChangeTime = h.ChangeTime,
                }).ToListAsync();
                // Log success
                _logger.LogInformation("Successfully fetched {Count} books with filtering: {Filter}, sorting: {Sort}, page: {PageSize}, size: {Size}, grouping: {GroupBy}",
                    historyDtos.Count, queryParams.Filter, queryParams.Sort, queryParams.Page, queryParams.PageSize, queryParams.GroupBy);
                
                return historyDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching with filtering: {Filter}, sorting: {Sort}, page: {PageSize}, size: {Size}, grouping: {GroupBy}",
                     queryParams.Filter, queryParams.Sort, queryParams.Page, queryParams.PageSize, queryParams.GroupBy);

                throw;
            }
        }

        public async Task<IEnumerable<ChangeResponseDto>> GetChangeHistoriesByIdAsync(Guid id)
        {
            try
            {
                var histories = await _unitOfWork.ChangeHistories.GetHistoriesByBookIdAsync(id);
                return histories.Select(h => new ChangeResponseDto
                {
                    Id = h.Id,
                    BookId = h.BookId,
                    ChangeDescription = h.ChangeDescription,
                    ChangeTime = h.ChangeTime,
                    BookTitle = h.Book.Title
                }).ToList();
            }
            catch (DbException ex)
            {
                _logger.LogError(ex,"An error occured while getting the history data");
                throw;
            }
            
        }
    }
}
