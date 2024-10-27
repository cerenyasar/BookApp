using BookAPI.Database;
using BookAPI.Dtos;
using BookAPI.Entities;
using BookAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;           
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
            var bookDto = new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                PublishDate = book.PublishDate,
                Authors = book.BookAuthors.Select(ba => ba.Author.Name).ToList()
            };
            return bookDto;
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

            // Filtering
            if (!string.IsNullOrEmpty(bookQueryParameters.Filter))
            {
                query = query.Where(b =>
                    b.Title.Contains(bookQueryParameters.Filter) ||
                    b.BookAuthors.Any(ba => ba.Author.Name.Contains(bookQueryParameters.Filter)));
            }

            //Sorting
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

            var bookDtos = await query.Select(book => new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                PublishDate = book.PublishDate,
                Authors = book.BookAuthors.Select(ba => ba.Author.Name).ToList()
            }).ToListAsync();

            return bookDtos;
        }

        public async Task<BookResponseDto> GetBookByIdAsync(Guid id)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} was not found.");

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


            if (!string.IsNullOrWhiteSpace(bookUpdateDto.Title) && bookUpdateDto.Title != "string")
            {
                book.Title = bookUpdateDto.Title;
            }

            if (!string.IsNullOrWhiteSpace(bookUpdateDto.Description) && bookUpdateDto.Description != "string")
            {
                book.Description = bookUpdateDto.Description;
            }

            if (bookUpdateDto.PublishDate.HasValue && bookUpdateDto.PublishDate.Value != new DateOnly(1, 1, 1))
            {
                book.PublishDate = bookUpdateDto.PublishDate.Value;
            }

            if(bookUpdateDto.Authors != null && bookUpdateDto.Authors.Any())
            {
                await UpdateBookAuthors(book, bookUpdateDto);
            }

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

        public async Task UpdateBookAuthors(Book book, BookUpdateDto bookUpdateDto)
        {
            // Handle authors
            var existingAuthors = book.BookAuthors.ToList();

            // Remove authors not in the update list
            foreach (var existingAuthor in existingAuthors)
            {
                if (!bookUpdateDto.Authors.Contains(existingAuthor.Author.Name))
                {
                    _unitOfWork.BookAuthors.RemoveBookAuthor(existingAuthor);
                }
            }

            // Add or retain authors
            foreach (var authorName in bookUpdateDto.Authors)
            {
                if(authorName != "string")
                {
                    var author = await _unitOfWork.Authors.GetAuthorByNameAsync(authorName) ?? new Author { Name = authorName };
                    await _unitOfWork.BookAuthors.AddBookAuthorAsync(book, author);
                }                                   
            }
        }
    }
}
