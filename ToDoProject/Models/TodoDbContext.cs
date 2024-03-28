using Microsoft.EntityFrameworkCore;
using ToDoProject.Controllers;

namespace ToDoProject.Models;
public class TodoDbContext : DbContext
{
    public TodoDbContext()
    {
            
    }
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {

    }

    public DbSet<Todo> Todos { get; set; }
    public DbSet<Category> Categories { get; set; }
}