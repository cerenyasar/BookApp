using BookAPI.Dtos;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeHistoryController : ControllerBase
    {
        private readonly IChangeHistoryService _changeHistoryService;
        private readonly ILogger<ChangeHistoryController> _logger;

        public ChangeHistoryController(IChangeHistoryService changeHistoryService, ILogger<ChangeHistoryController> logger)
        {
            _changeHistoryService = changeHistoryService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHistoriesByBookId(Guid id)
        {
            try
            {
                var histories = await _changeHistoryService.GetChangeHistoriesByIdAsync(id);
                return Ok(histories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting histories for book {id}", id);
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHistories([FromQuery] ChangeHistoryQueryParams queryParameters)
        {            
            try
            {
                var books = await _changeHistoryService.GetAllChangeHistoriesAsync(queryParameters);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting histories");
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }
    }
}
