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
            return await _context.Authors.ToListAsync<Author>();
        }

        public async Task<Author?> GetAuthorByIdAsync(Guid authorId)
        {
            return await _context.Authors.FindAsync(authorId);
        }

        public Task<Author?> GetAuthorByNameAsync(string name)
        {
            return _context.Authors.Where(a => a.Name == name).FirstOrDefaultAsync();
        }

    }
}
