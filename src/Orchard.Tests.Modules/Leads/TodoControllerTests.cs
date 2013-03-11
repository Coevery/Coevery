using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using Autofac;
using Coevery.Leads.Controllers;
using Coevery.Leads.Services;

using NUnit.Framework;

using Orchard.Data;

using Coevery.Leads.Models;


namespace Orchard.Tests.Modules.Leads
{
    [TestFixture]
    public class TodoControllerTests : DatabaseEnabledTestsBase
    {
        private TodoController _todoController;
        private TodoListController _todoListController;

        public override void Register(ContainerBuilder builder)
        {
            builder.RegisterType<TodoService>().As<ITodoService>();
            builder.RegisterType<TodoController>().SingleInstance();
            builder.RegisterType<TodoListController>().SingleInstance();
       
        
            builder.RegisterType<Repository<TodoListRecord>>().As<IRepository<TodoListRecord>>();
            builder.RegisterType<Repository<TodoItemRecord>>().As<IRepository<TodoItemRecord>>();
        }

        protected override IEnumerable<Type> DatabaseTypes
        {
            get
            {
                return new[] { typeof(TodoItemRecord),
                    typeof(TodoListRecord)
                };
            }
        }

        public override void Init()
        {
            base.Init();
            _todoController = _container.Resolve<TodoController>();
            _todoListController = _container.Resolve<TodoListController>();
            _todoController.SetUserId("admin");
            _todoListController.SetUserId("admin");
        }

        [Test]
        public void TodoItemCrud()
        {
            //Add one todo List.
            TodoListDto todoListDto = new TodoListDto();
            todoListDto.Title = "todoList1";
            todoListDto.UserId = "admin";

            FakeRequest(_todoListController, HttpMethod.Post, "");
            // enable user registration
            var reAddTodoList = _todoListController.PostTodoList(todoListDto);
            Assert.That(reAddTodoList.IsSuccessStatusCode, Is.EqualTo(true));
            var response = reAddTodoList.Content.ReadAsStringAsync();
            var jsonObjResponse = Json.Decode(response.Result.ToString());
            int todoListId = jsonObjResponse.TodoListId;
            string title = jsonObjResponse.Title;
            string userId = jsonObjResponse.UserId;
            Assert.That(title, Is.EqualTo(todoListDto.Title));
            Assert.That(userId, Is.EqualTo(todoListDto.UserId));
            Assert.That(todoListId, Is.GreaterThan(0));

            //Add one todo item.
            TodoItemDto todoItemDto = new TodoItemDto
            {
                IsDone = true,
                Title = "todoItemTest1",
                TodoListId = todoListDto.TodoListId
            };
            FakeRequest(_todoController, HttpMethod.Post, "");
            var reAddtodo = _todoController.PostTodoItem(todoItemDto);
            Assert.That(reAddtodo.IsSuccessStatusCode,Is.EqualTo(true));
            Assert.That(todoItemDto.TodoItemId,Is.GreaterThan(0));

            //Get todoItem 
            FakeRequest(_todoListController, HttpMethod.Get, "");
            var reTodoList =  _todoListController.GetTodoList(todoListId);
            Assert.That(reTodoList.Todos.Count,Is.EqualTo(1));
            Assert.That(reTodoList.Todos[0].Title, Is.EqualTo("todoItemTest1"));
            Assert.That(reTodoList.Todos[0].IsDone, Is.EqualTo(true));
            Assert.That(reTodoList.Todos[0].TodoListId, Is.EqualTo(todoListId));
            int todoItemId = reTodoList.Todos[0].TodoItemId;
            Assert.That(todoItemId,Is.GreaterThan(0));

            //Put todoItem
            TodoItemDto todoItemDtoModify = reTodoList.Todos[0];
            todoItemDtoModify.Title = "todoItemTest2";
            todoItemDtoModify.IsDone = false;
            FakeRequest(_todoListController, HttpMethod.Put, "");
            var reModifyTodoItem = _todoController.PutTodoItem(todoItemId, todoItemDtoModify);
            Assert.That(reModifyTodoItem.IsSuccessStatusCode,Is.EqualTo(true));

            //Get todoItem
            FakeRequest(_todoListController, HttpMethod.Get, "");
            var reTodoList2 = _todoListController.GetTodoList(todoListId);
            Assert.That(reTodoList2.Todos.Count, Is.EqualTo(1));
            Assert.That(reTodoList2.Todos[0].Title, Is.EqualTo("todoItemTest2"));
            Assert.That(reTodoList2.Todos[0].IsDone, Is.EqualTo(false));
           
            //Delete todoItem
            FakeRequest(_todoController, HttpMethod.Delete, "");
            var reDeleteTodoItem = _todoController.DeleteTodoItem(todoItemId);
            Assert.That(reDeleteTodoItem.IsSuccessStatusCode,Is.EqualTo(true));

            //GetTodoItem
            FakeRequest(_todoListController, HttpMethod.Get, "");
            var reTodoList3 = _todoListController.GetTodoList(todoListId);
            Assert.That(reTodoList3.Todos.Count, Is.EqualTo(0));
        }




        public void FakeRequest(ApiController controller,
            HttpMethod method = null,
            string requestUrl = null,
            string controllerName = null)
        {
            HttpConfiguration config = new HttpConfiguration();
            // rebuild the expected request
            var request = new HttpRequestMessage(method, requestUrl);
            //var route = System.Web.Routing.RouteTable.Routes["DefaultApi"];
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");

            // TODO: get from application?  maybe like http://stackoverflow.com/a/5943810/1037948
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", controllerName } });

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);

            // attach fake request
            controller.Request = request;
            controller.Request.Properties[/* "MS_HttpConfiguration" */ HttpPropertyKeys.HttpConfigurationKey] = config;
        }


    }
}
