using BookAPI.Dtos;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] BookQueryParameters queryParameters)
        {
            var books = await _bookService.GetAllBooksAsync(queryParameters);
            return Ok(books);
        }        

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] BookCreateDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            try
            {
                var createdBook = await _bookService.AddBookAsync(bookDto);
                return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new book.");
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookUpdateDto bookDto)
        {
            try
            {
                var updatedBook = await _bookService.UpdateBookAsync(id, bookDto);
                return Ok(updatedBook); 
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Book with ID {Id} not found", id);
                return NotFound($"Book with ID {id} was not found."); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the book with ID {Id}", id);
                return StatusCode(500, "An internal error occurred. Please try again later."); 
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            try
            {
                await _bookService.DeleteBookAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the book.", error = ex.Message });
            }
        }
    }
}
