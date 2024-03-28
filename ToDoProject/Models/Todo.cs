using System.ComponentModel.DataAnnotations;

namespace ToDoProject.Models;

public class Todo
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime DueDate { get; set; }
    
    public virtual Category Category { get; set; }
}