using System.ComponentModel.DataAnnotations;


namespace Coevery.Leads.Models
{
    /// <summary>
    /// Todo item entity
    /// </summary>
    public class TodoItemRecord
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual string Title { get; set; }
        public virtual bool IsDone { get; set; }
       
        public virtual TodoListRecord TodoList { get; set; }
    }
}