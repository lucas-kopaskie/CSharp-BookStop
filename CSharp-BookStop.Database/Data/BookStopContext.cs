using CSharp_BookStop.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.Database.Data;

public class BookStopContext(DbContextOptions<BookStopContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Author>  Authors { get; set; }
    public DbSet<Genre>  Genres { get; set; }
    public DbSet<Book>  Books { get; set; }
    public DbSet<Cart>  Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
}