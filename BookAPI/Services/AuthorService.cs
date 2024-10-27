using BookAPI.Database;
using BookAPI.Dtos;
using BookAPI.Entities;
using BookAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookAPI.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthorResponseDto> AddAuthorAsync(AuthorDto authorCreateDto)
        {
            var author = new Author
            {
                Name = authorCreateDto.Name,
                BookAuthors = new HashSet<BookAuthor>(),
            };

            _unitOfWork.Authors.Add(author);

            await _unitOfWork.CompleteAsync();
            return new AuthorResponseDto
            {
                Id = author.Id,
                Name = author.Name,
            };

        }

        public async Task DeleteAuthorAsync(Guid id)
        {
            var author = await _unitOfWork.Authors.GetAuthorByIdAsync(id);
            if (author == null)
                throw new KeyNotFoundException($"Author with ID {id} was not found.");

            // Handle BookAuthor connection
            var existingBooks = author.BookAuthors.ToList();

            foreach (var existingBook in existingBooks)
            {                
                _unitOfWork.BookAuthors.RemoveBookAuthor(existingBook);                
            }

            _unitOfWork.Authors.Delete(author);
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while deleting the author.", ex);
            }
        }

        public async Task<IEnumerable<AuthorResponseDto>> GetAllAuthorsAsync()
        {
            var authors = await _unitOfWork.Authors.GetAllAuthorsAsync();
            var authorDtos = authors.Select(author => new AuthorResponseDto
            {
                Id = author.Id,
                Name = author.Name,
                Books = author.BookAuthors.Select(ba => ba.Book.Title).ToList()
            }).ToList();
            return authorDtos;

        }

        public async Task<AuthorResponseDto> GetAuthorByIdAsync(Guid id)
        {            
            var author = await _unitOfWork.Authors.GetAuthorByIdAsync(id);
            if (author == null)
                throw new KeyNotFoundException($"Author with ID {id} was not found.");

            return new AuthorResponseDto
            {
                Id = author.Id,
                Name = author.Name,
                Books = author.BookAuthors.Select(ba => ba.Book.Title).ToList()
            };
        }

        public async Task<AuthorResponseDto> GetAuthorByNameAsync(string name)
        {
            var author = await _unitOfWork.Authors.GetAuthorByNameAsync(name);
            if (author == null)
                throw new KeyNotFoundException($"Author with Name {name} was not found.");

            return new AuthorResponseDto
            {
                Id = author.Id,
                Name = author.Name,
                Books = author.BookAuthors.Select(ba => ba.Book.Title).ToList()
            };
        }

        public async Task<AuthorResponseDto> UpdateAuthorAsync(Guid id, AuthorDto authorDto)
        {
            var author = await _unitOfWork.Authors.GetAuthorByIdAsync(id);
            if(author == null)
                throw new KeyNotFoundException($"Author with ID {id} was not found.");

            if (!string.IsNullOrWhiteSpace(authorDto.Name) && authorDto.Name != "string")
            {
                author.Name = authorDto.Name;
            }

            await _unitOfWork.CompleteAsync();
            return new AuthorResponseDto
            {
                Id = author.Id,
                Name = author.Name,
                Books = author.BookAuthors.Select(ba => ba.Book.Title).ToList()
            };
        }
    
    }
}
