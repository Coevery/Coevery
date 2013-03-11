using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Coevery.Leads.Models
{
    /// <summary>
    /// Data transfer object for <see cref="TodoListRecord"/>
    /// </summary>
    public class TodoListDto
    {
        public TodoListDto()
        {
            Todos = new List<TodoItemDto>();
        }

        public TodoListDto(TodoListRecord todoList)
        {
            TodoListId = todoList.Id;
            UserId = todoList.UserId;
            Title = todoList.Title;
            Todos = new List<TodoItemDto>();
            foreach (TodoItemRecord item in todoList.Todos)
            {
                Todos.Add(new TodoItemDto(item));
            }
        }

        [Key]
        public int TodoListId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Title { get; set; }

        public virtual List<TodoItemDto> Todos { get; set; }

        public TodoListRecord ToEntity()
        {
            TodoListRecord todo = new TodoListRecord
            {
                Title = Title,
                Id = TodoListId,
                UserId = UserId,
                Todos = new List<TodoItemRecord>()
            };
            foreach (TodoItemDto item in Todos)
            {
                todo.Todos.Add(item.ToEntity());
            }

            return todo;
        }
    }
}