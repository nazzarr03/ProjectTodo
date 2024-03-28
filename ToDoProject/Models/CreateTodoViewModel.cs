namespace ToDoProject.Models
{
    public class CreateTodoViewModel
    {
        public Todo Todo { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}