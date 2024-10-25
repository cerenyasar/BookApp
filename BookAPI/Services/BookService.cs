using BookAPI.Database;
using BookAPI.Dtos;
using BookAPI.Entities;
using BookAPI.Services.Interfaces;

namespace BookAPI.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;           
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _unitOfWork.Books.GetAllBooksAsync();
        }

        public async Task<Book?> GetBookByIdAsync(Guid id)
        {
            return await _unitOfWork.Books.GetBookByIdAsync(id);
        }

        public async Task<Book> AddBookAsync(BookCreateDto bookCreateDto) 
        {
            var book = new Book
            {
                Title = bookCreateDto.Title,
                Description = bookCreateDto.Description,
                PublishDate = bookCreateDto.PublishedDate,
                BookAuthors = new HashSet<BookAuthor>(),
            };

            _unitOfWork.Books.Add(book);

            foreach (var authorName in bookCreateDto.Authors)
            {
                var author = await _unitOfWork.Authors.GetAuthorByNameAsync(authorName);
                if (author == null)
                {
                    author = new Author { Name = authorName, BookAuthors = new HashSet<BookAuthor>()};
                    _unitOfWork.Authors.Add(author);
                }
                var bookAuthor = new BookAuthor { Author = author, Book = book };
                if (!book.BookAuthors.Any(ba => ba.Author.Id == author.Id && ba.Book.Id == book.Id))
                {
                    book.BookAuthors.Add(bookAuthor);
                }

                if (!author.BookAuthors.Any(ba => ba.Book.Id == book.Id && ba.Author.Id == author.Id))
                {
                    author.BookAuthors.Add(bookAuthor);
                }
            }

            await _unitOfWork.CompleteAsync();
            return book;
        }

    }
}
