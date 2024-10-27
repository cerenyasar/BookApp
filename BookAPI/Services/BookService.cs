using BookAPI.Database;
using BookAPI.Dtos;
using BookAPI.Entities;
using BookAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BookAPI.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BookService> _logger;

        public BookService(IUnitOfWork unitOfWork, ILogger<BookService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BookResponseDto> AddBookAsync(BookCreateDto bookCreateDto)
        {
            var book = new Book
            {
                Title = bookCreateDto.Title,
                Description = bookCreateDto.Description,
                PublishDate = bookCreateDto.PublishDate,
                BookAuthors = new HashSet<BookAuthor>(),
            };

            try
            {
                _unitOfWork.Books.Add(book);

                foreach (var authorName in bookCreateDto.Authors)
                {
                    var author = await _unitOfWork.Authors.GetAuthorByNameAsync(authorName);
                    if (author == null)
                    {
                        author = new Author { Name = authorName, BookAuthors = new HashSet<BookAuthor>() };
                        _unitOfWork.Authors.Add(author);
                    }

                    // Manage the relationship
                    await _unitOfWork.BookAuthors.AddBookAuthorAsync(book, author);
                }

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Successfully added book with ID {BookId} and Name {BookTitke}", book.Id, book.Title);

                return new BookResponseDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    PublishDate = book.PublishDate,
                    Authors = book.BookAuthors.Select(ba => ba.Author.Name).ToList()
                };
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occurred while adding an book: {BookTitke}", bookCreateDto.Title);
                throw;
            }            
        }

        public async Task DeleteBookAsync(Guid id)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} was not found.");

            // Handle BookAuthor connection
            var existingAuthors = book.BookAuthors.ToList();

            foreach (var existingAuthor in existingAuthors)
            {
                _unitOfWork.BookAuthors.RemoveBookAuthor(existingAuthor);
            }

            _unitOfWork.Books.Delete(book);
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while deleting the author.", ex);
            }
        }

        public async Task<IEnumerable<BookResponseDto>> GetAllBooksAsync(BookQueryParameters bookQueryParameters)
        {
            IQueryable<Book> query = _unitOfWork.Books.GetAllBooks();

            try
            {
                // Filtering
                if (!string.IsNullOrEmpty(bookQueryParameters.Filter))
                {
                    query = query.Where(b =>
                        b.Title.Contains(bookQueryParameters.Filter) ||
                        b.BookAuthors.Any(ba => ba.Author.Name.Contains(bookQueryParameters.Filter)));
                }

                // Sorting
                if (!string.IsNullOrEmpty(bookQueryParameters.Sort))
                {
                    query = bookQueryParameters.Sort.ToLower() switch
                    {
                        "title" => query.OrderBy(b => b.Title),
                        "publishdate" => query.OrderBy(b => b.PublishDate),
                        _ => query // No sorting if invalid parameter
                    };
                }

                // Pagination
                var skip = (bookQueryParameters.Page - 1) * bookQueryParameters.Size;
                query = query.Skip(skip).Take(bookQueryParameters.Size);

                // Fetching data
                var bookDtos = await query.Select(book => new BookResponseDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    PublishDate = book.PublishDate,
                    Authors = book.BookAuthors.Select(ba => ba.Author.Name).ToList()
                }).ToListAsync();

                // Log success
                _logger.LogInformation("Successfully fetched {Count} books with filtering: {Filter}, sorting: {Sort}, page: {Page}, size: {Size}",
                    bookDtos.Count, bookQueryParameters.Filter, bookQueryParameters.Sort, bookQueryParameters.Page, bookQueryParameters.Size);

                return bookDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching books with filtering: {Filter}, sorting: {Sort}, page: {Page}, size: {Size}",
                    bookQueryParameters.Filter, bookQueryParameters.Sort, bookQueryParameters.Page, bookQueryParameters.Size);

                throw;
            }
        }

        public async Task<BookResponseDto> GetBookByIdAsync(Guid id)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(id);
            if (book == null)
            {
                _logger.LogError($"Book with ID {id} was not found.");
                throw new KeyNotFoundException($"Book with ID {id} was not found.");
            }

            return new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                PublishDate = book.PublishDate,
                Authors = book.BookAuthors.Select(ba => ba.Author.Name).ToList()
            };
        }

        public async Task<BookResponseDto> UpdateBookAsync(Guid id, BookUpdateDto bookUpdateDto)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} was not found.");

            var description = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(bookUpdateDto.Title) && bookUpdateDto.Title != "string")
            {

                description.Append($"Title changed from \"{book.Title}\" to \"{bookUpdateDto.Title}\"");
                book.Title = bookUpdateDto.Title;
            }

            if (!string.IsNullOrWhiteSpace(bookUpdateDto.Description) && bookUpdateDto.Description != "string")
            {
                description.Append($"Description changed from \"{book.Description}\" to \"{bookUpdateDto.Description}\"");
                book.Description = bookUpdateDto.Description;
            }

            if (bookUpdateDto.PublishDate.HasValue && bookUpdateDto.PublishDate.Value != new DateOnly(1, 1, 1))
            {
                description.Append($"PublishDate changed from \"{book.PublishDate}\" to \"{bookUpdateDto.PublishDate.Value}\"");
                book.PublishDate = bookUpdateDto.PublishDate.Value;
            }

            if(bookUpdateDto.Authors != null && bookUpdateDto.Authors.Any() && !bookUpdateDto.Authors.Contains("string"))
            {
                await UpdateBookAuthors(book, bookUpdateDto, description);
            }

            AddChangeHistory(book, description.ToString());

            try
            {
                await _unitOfWork.CompleteAsync();
                return new BookResponseDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    PublishDate = book.PublishDate,
                    Authors = book.BookAuthors.Select(ba => ba.Author.Name).ToList()
                };
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occurred while updating the book: {BookId}", book.Id);
                throw;
            }
            
        }

        public async Task UpdateBookAuthors(Book book, BookUpdateDto bookUpdateDto, StringBuilder description)
        {
            // Handle authors
            var existingAuthors = book.BookAuthors.ToList();

            // Remove authors not in the update list
            foreach (var existingAuthor in existingAuthors)
            {
                if (!bookUpdateDto.Authors.Contains(existingAuthor.Author.Name))
                {
                    description.Append($"\"{existingAuthor.Author.Name}\" removed from author list");
                    _unitOfWork.BookAuthors.RemoveBookAuthor(existingAuthor);
                }
            }

            // Add or retain authors
            foreach (var authorName in bookUpdateDto.Authors)
            {
                if(authorName != "string")
                {
                    description.Append($"\"{authorName}\" added to author list");
                    var author = await _unitOfWork.Authors.GetAuthorByNameAsync(authorName) ?? new Author { Name = authorName };
                    await _unitOfWork.BookAuthors.AddBookAuthorAsync(book, author);
                }                                   
            }
        }

        public void AddChangeHistory(Book book, string description)
        {
            var changeHistory = new ChangeHistory
            {
                Book = book,
                BookId = book.Id,
                ChangeTime = DateTime.Now,
                ChangeDescription = description,
            };

            _unitOfWork.ChangeHistories.Add(changeHistory);
        }
    }
}
