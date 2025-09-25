using CSharp_BookStop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.API.Data;

public class BookStopContext(DbContextOptions<BookStopContext> options) : DbContext(options)
{
    public DbSet<Author>  Authors { get; set; }
    public DbSet<Genre>  Genres { get; set; }
    public DbSet<Book>  Books { get; set; }
    public DbSet<User>  Users { get; set; }
    public DbSet<UserCart>  UserCarts { get; set; }
    public DbSet<UserCartItem> UserCartItems { get; set; }
}