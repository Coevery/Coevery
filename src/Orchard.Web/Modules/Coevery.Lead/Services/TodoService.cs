using System.Data.Entity;
using Coevery.Leads.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Data;

namespace Coevery.Leads.Services
{
    public  interface  ITodoService : IDependency
    {
        IEnumerable<TodoListDto> GetAllTodoListDto();

        TodoListDto GetTodoListDto(int id);
        bool AddTodoList(TodoListDto todoListDto);
        bool UpdateTodoList(int id, TodoListDto todoListDto);
        bool DeleteTodoList(int id);

        TodoItemRecord GetTodoItem(int id);
        bool UpdateTodoItem(int id, TodoItemDto todoItemDto);
        bool AddTodoItem(TodoItemDto todoItemDto);
        bool DeleteTodoItem(TodoItemDto todoItemDto);
    }

    public class TodoService : ITodoService
    {
        private IRepository<TodoListRecord> _todoListRepository;
        private IRepository<TodoItemRecord> _todoItemRepository;

        public TodoService(IRepository<TodoListRecord> todoListRepository,
            IRepository<TodoItemRecord> todoItemRepository)
        {
            _todoListRepository = todoListRepository;
            _todoItemRepository = todoItemRepository;
        }
        public IEnumerable<TodoListDto> GetAllTodoListDto()
        {
            List<TodoListDto> reDtos = new List<TodoListDto>();
            var re = _todoListRepository.Table.AsEnumerable();
            reDtos.AddRange(re.Select(todoList => new TodoListDto(todoList)));
            return reDtos;
        }

        public TodoListDto GetTodoListDto(int id)
        {
            TodoListRecord todoList = _todoListRepository.Get(id);
            if (todoList != null)
            {
                return new TodoListDto(todoList);
            }
            else
            {
                return null;
            }
        }

        public bool AddTodoList(TodoListDto todoListDto)
        {
            TodoListRecord todoList = todoListDto.ToEntity();
            _todoListRepository.Create(todoList);
            _todoListRepository.Flush();
            todoListDto.TodoListId = todoList.Id;
            return true;
        }

        public bool UpdateTodoList(int id, TodoListDto todoListDto)
        {
            TodoListRecord todoListRecord = _todoListRepository.Get(id);
            todoListRecord.Title = todoListDto.Title;
            _todoListRepository.Flush();
            return true;
        }

        public bool DeleteTodoList(int id)
        {
            var todoList = _todoListRepository.Get(id);
            if (todoList == null) return false;

            _todoListRepository.Delete(todoList);
            _todoListRepository.Flush();
            return true;
        }

        public bool UpdateTodoItem(int id, TodoItemDto todoItemDto)
        {
            TodoItemRecord todoItem = todoItemDto.ToEntity();
            TodoItemRecord memoryItemRecord = _todoItemRepository.Get(todoItem.Id);
            if (memoryItemRecord != null)
            {
                memoryItemRecord.IsDone = todoItem.IsDone;
                memoryItemRecord.Title = todoItem.Title;
            }
            else
            {
                return false;
            }
            _todoItemRepository.Flush();
            return true;
        }

        public bool AddTodoItem(TodoItemDto todoItemDto)
        {
            TodoItemRecord todoItem = todoItemDto.ToEntity();

            TodoListRecord todoList = _todoListRepository.Get(todoItemDto.TodoListId);
            todoItem.TodoList = todoList;
            todoList.Todos.Add(todoItem);
            _todoListRepository.Flush();
            todoItemDto.TodoItemId = todoItem.Id;
            return true;
        }

        public bool DeleteTodoItem(TodoItemDto todoItemDto)
        {
            TodoItemRecord todoItem = _todoItemRepository.Get(todoItemDto.TodoItemId);
            todoItem.TodoList.Todos.Remove(todoItem);
            _todoItemRepository.Flush();
            return true;
        }

        public TodoItemRecord GetTodoItem(int id)
        {
            TodoItemRecord todoItemRecord = _todoItemRepository.Get(id);
            return todoItemRecord;
        }
    }
}