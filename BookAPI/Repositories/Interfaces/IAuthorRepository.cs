using BookAPI.Entities;

namespace BookAPI.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author?> GetAuthorByIdAsync(Guid authorId);
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author?> GetAuthorByNameAsync(string name);


        void Add(Author author);
        void Update(Author author);
        void Delete(Author author);
    }
}
