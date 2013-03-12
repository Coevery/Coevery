using System.ComponentModel.DataAnnotations;

namespace Coevery.Leads.Models
{
    public class TodoItemDto
    {
        /// <summary>
        /// Data transfer object for <see cref="TodoItemRecord"/>
        /// </summary>
        public TodoItemDto() { }

        public TodoItemDto(TodoItemRecord item)
        {
            TodoItemId = item.Id;
            Title = item.Title;
            IsDone = item.IsDone;
            TodoListId = item.TodoList.Id;
        }

        [Key]
        public int TodoItemId { get; set; }

        [Required]
        public string Title { get; set; }

        public bool IsDone { get; set; }

        public int TodoListId { get; set; }

        public TodoItemRecord ToEntity()
        {
            return new TodoItemRecord
            {
                Id = TodoItemId,
                Title = Title,
                IsDone = IsDone
            };
        }
    }
}
