using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Leads.Models;
using Coevery.Leads.Services;

namespace Coevery.Leads.Controllers
{
    [Authorize]
    public class TodoListController : ApiController
    {
        private ITodoService _service;
        private string _userId;

        public TodoListController(ITodoService service)
        {
            _service = service;
            _userId = User.Identity.Name;
        }

        public void SetUserId(string userId)
        {
            _userId = userId;
        }

        // GET api/TodoList
        public IEnumerable<TodoListDto> GetTodoLists()
        {
            try
            {
                return _service.GetAllTodoListDto();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET api/TodoList/5
        public TodoListDto GetTodoList(int id)
        {
            TodoListDto todoListDto = _service.GetTodoListDto(id);
            if (todoListDto == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            //if (todoListDto.UserId != _userId)
            //{
            //    // Trying to modify a record that does not belong to the user
            //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Unauthorized));
            //}

            return todoListDto;
        }

        // PUT api/TodoList/5
        //[ValidateHttpAntiForgeryToken]
        public HttpResponseMessage PutTodoList(int id, TodoListDto todoListDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != todoListDto.TodoListId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            TodoListRecord todoList = todoListDto.ToEntity();
            //todo patch

            try
            {
                _service.UpdateTodoList(id, todoListDto);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/TodoList
        //[ValidateHttpAntiForgeryToken]
        public HttpResponseMessage PostTodoList(TodoListDto todoListDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            todoListDto.UserId = _userId;
            _service.AddTodoList(todoListDto);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, todoListDto);
            //response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = todoListDto.TodoListId }));
            return response;
        }

        // DELETE api/TodoList/5
        //[ValidateHttpAntiForgeryToken]
        public HttpResponseMessage DeleteTodoList(int id)
        {
            TodoListDto todoListDto = _service.GetTodoListDto(id);
            if (todoListDto == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            try
            {
                _service.DeleteTodoList(id);
               
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, todoListDto);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}