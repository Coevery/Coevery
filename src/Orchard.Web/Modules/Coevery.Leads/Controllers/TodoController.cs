using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Leads.Models;
using Coevery.Leads.Services;


namespace Coevery.Leads.Controllers
{
    [Authorize]
    //[ValidateHttpAntiForgeryToken]
    public class TodoController : ApiController
    {
        private ITodoService _todoService;
        private string _userId;

        public TodoController(ITodoService todoService )
        {
            _todoService = todoService;
        }

        public void SetUserId(string userId)
        {
            _userId = userId;
        }

        // PUT api/Todo/5
        public HttpResponseMessage PutTodoItem(int id, TodoItemDto todoItemDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != todoItemDto.TodoItemId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            TodoItemRecord todoItem = todoItemDto.ToEntity();
            TodoListDto todoList = _todoService.GetTodoListDto(id);
            if (todoList == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (todoList.UserId != _userId)
            {
                // Trying to modify a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            // Need to detach to avoid duplicate primary key exception when SaveChanges is called
          

            try
            {
                bool success = _todoService.UpdateTodoItem(id, todoItemDto);
                if (!success)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Todo
        public HttpResponseMessage PostTodoItem(TodoItemDto todoItemDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            TodoListDto todoList = _todoService.GetTodoListDto(todoItemDto.TodoListId);
            if (todoList == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (todoList.UserId != _userId)
            {
                // Trying to add a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            _todoService.AddTodoItem(todoItemDto);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, todoItemDto);
            //response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = todoItemDto.TodoItemId }));
            return response;
        }

        // DELETE api/Todo/5
        public HttpResponseMessage DeleteTodoItem(int id)
        {
            TodoItemRecord todoItem = _todoService.GetTodoItem(id);
            if (todoItem == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (todoItem.TodoList.UserId != _userId)
            {
                // Trying to delete a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            TodoItemDto todoItemDto = new TodoItemDto(todoItem);

            try
            {
                _todoService.DeleteTodoItem(todoItemDto);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, todoItemDto);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}