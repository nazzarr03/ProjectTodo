namespace ToDoProject.Models;

public class CreateTodoModel
{

    public string Title { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime DueDate { get; set; }
    
    public virtual int Category { get; set; }
}