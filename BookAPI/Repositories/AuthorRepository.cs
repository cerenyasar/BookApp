using BookAPI.Database;
using BookAPI.Entities;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly BookDbContext _context;

        public AuthorRepository(BookDbContext context)
        {
            _context = context;
        }

        public void Add(Author author) => _context.Authors.Add(author);
        public void Update(Author author) => _context.Authors.Update(author);
        public void Delete(Author author) => _context.Authors.Remove(author);


        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors.Include(a => a.BookAuthors)
                                   .ThenInclude(ba => ba.Book)
                                   .ToListAsync();
        }

        public async Task<Author?> GetAuthorByIdAsync(Guid authorId)
        {
            return await _context.Authors.Include(a => a.BookAuthors)
                                   .ThenInclude(ba => ba.Book)
                                   .FirstOrDefaultAsync(a => a.Id == authorId);
        }

        public async Task<Author?> GetAuthorByNameAsync(string name)
        {
            return await _context.Authors.Include(a => a.BookAuthors)
                                   .ThenInclude(ba => ba.Book)
                                   .FirstOrDefaultAsync(a => a.Name == name);
        }

    }
}
