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
using Moq;
using NUnit.Framework;
using Orchard.Caching;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Services;
using Orchard.Core.Settings.Metadata;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Implementation;
using Orchard.Environment;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Messaging.Events;
using Orchard.Messaging.Services;
using Orchard.Security;
using Orchard.Security.Permissions;
using Orchard.Security.Providers;
using Orchard.Tests.Modules.Users;
using Orchard.Tests.Stubs;
using Orchard.UI.Notify;
using Orchard.Users.Controllers;
using Orchard.Users.Handlers;
using Orchard.Users.Models;
using Orchard.Users.Services;
using Orchard.Settings;
using Orchard.Core.Settings.Services;
using Orchard.Tests.Messaging;
using Orchard.Core.Settings.Models;
using Orchard.Core.Settings.Handlers;
using System.Collections.Specialized;

using Coevery.Leads.Models;
using System.Net;

namespace Orchard.Tests.Modules.Leads
{
    [TestFixture]
    public class TodoListsControllerTests : DatabaseEnabledTestsBase
    {
        private TodoListController _controller;

        public override void Register(ContainerBuilder builder)
        {
            builder.RegisterType<TodoService>().As<ITodoService>();
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
            _controller = _container.Resolve<TodoListController>();
            _controller.SetUserId("admin");
    
            
        }

        private void DoWebRequest(string url)
        {
            WebRequest req = WebRequest.Create(url);
            WebResponse res = req.GetResponse();
        }

        [Test]
        public void TodoListCrud()
        {
            TodoListDto todoListDto = new TodoListDto();
            todoListDto.Title = "todoList1";
            todoListDto.UserId = "admin";

          
            //PostTest
            FakeRequest(_controller, HttpMethod.Post, "");
            // enable user registration
            var re = _controller.PostTodoList(todoListDto);
            Assert.That(re.IsSuccessStatusCode, Is.EqualTo(true));
            var response = re.Content.ReadAsStringAsync();
            var  jsonObjResponse = Json.Decode(response.Result.ToString());
            int todoListId = jsonObjResponse.TodoListId;
            string title = jsonObjResponse.Title;
            string userId = jsonObjResponse.UserId;
            Assert.That(title, Is.EqualTo(todoListDto.Title));
            Assert.That(userId, Is.EqualTo(todoListDto.UserId));

            //GetTest
            FakeRequest(_controller, HttpMethod.Get, "");
            var re4 = _controller.GetTodoList(todoListId);
            Assert.That(re4, Is.Not.Null);
            Assert.That(re4.Title, Is.EqualTo("todoList1"));
            Assert.That(re4.UserId, Is.EqualTo("admin"));

            //PutTest
            FakeRequest(_controller, HttpMethod.Put, "");
            todoListDto.Title = "ModifyedTitle";
            var re2 = _controller.PutTodoList(todoListDto.TodoListId, todoListDto);
            Assert.That(re2.IsSuccessStatusCode, Is.EqualTo(true));

            //GetTest
            FakeRequest(_controller, HttpMethod.Get, "");
            var re3 = _controller.GetTodoList(todoListId);
            Assert.That(re3, Is.Not.Null);
            Assert.That(re3.Title, Is.EqualTo("ModifyedTitle"));

            //DeleteTest
            FakeRequest(_controller, HttpMethod.Delete, "");
            var re5 = _controller.DeleteTodoList(todoListId);
            Assert.That(re5.IsSuccessStatusCode, Is.EqualTo(true));

            //GetTest
            FakeRequest(_controller, HttpMethod.Get, "");
            try
            {
                var re6 = _controller.GetTodoList(todoListId);
                Assert.Fail();
            }
            catch (HttpResponseException ex)
            {
               Assert.That(ex.Response.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
            }
           
            
        }

        [Test]
        public void TodoListMutilAddAndGetAll()
        {
            List<TodoListDto> todoListDtos = new List<TodoListDto>();
            for (int index = 0; index < 5; index ++)
            {
                TodoListDto todoListDto = new TodoListDto();
                todoListDto.Title = "todoList" + index.ToString();
                todoListDto.UserId = "admin";
                todoListDtos.Add(todoListDto);
            }
            //PostTest
            FakeRequest(_controller, HttpMethod.Post, "");
            // enable user registration
            foreach (var todoListDto in todoListDtos)
            {
                var re = _controller.PostTodoList(todoListDto);
                Assert.That(re.IsSuccessStatusCode, Is.EqualTo(true));
            }

            //GetAllTest
            FakeRequest(_controller, HttpMethod.Get, "");
            var todoListsRe = _controller.GetTodoLists();
            List<TodoListDto> todoListReList = new List<TodoListDto>(todoListsRe);
            Assert.That(todoListReList.Count,Is.EqualTo(5));
            foreach (var todoListDto in todoListReList)
            {
                Assert.That(todoListDto.UserId,Is.EqualTo("admin"));
            }

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
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary
            {
                { "controller", controllerName }
            });

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);

            // attach fake request
            controller.Request = request;
            controller.Request.Properties[ HttpPropertyKeys.HttpConfigurationKey] = config;
        }
        

    }
}
