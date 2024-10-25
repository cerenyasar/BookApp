using BookAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Database
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<ChangeHistory> ChangeHistories { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-many relationship for Books and Authors
            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(ba => ba.BookId);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(ba => ba.AuthorId);

            // One-to-many relationship for Book and ChangeHistory
            modelBuilder.Entity<ChangeHistory>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.ChangeHistories)
                .HasForeignKey(bc => bc.BookId);

            // Author name is unique
            modelBuilder.Entity<Author>()
                .HasIndex(a => a.Name)
                .IsUnique();
        }
    }
}

