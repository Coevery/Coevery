using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Orchard.ContentManagement;

namespace Orchard.WebApi.Common
{
    public abstract class ContentController<TContentType, TContentDtoType> : ApiController
        where TContentType : ContentPart
        where TContentDtoType : IDto<TContentType>
    {
        protected readonly IContentManager _contentManager;

        public ContentController(IOrchardServices services, IContentManager contentManager)
        {
            Services = services;
            _contentManager = contentManager;
        }

        public IOrchardServices Services { get; set; }

        // GET api/leads/lead
        public virtual IEnumerable<TContentDtoType> GetContents()
        {
            var reDtos = new List<TContentDtoType>();
            var re = _contentManager.List<TContentType>();
            reDtos.AddRange(re.Select(record => ConstructDto(record)));
            return reDtos;
        }

        // GET api/leads/lead/5
        public virtual TContentDtoType GetContent(int id)
        {
            var content = _contentManager.Get<TContentType>(id);
            if (content == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return ConstructDto(content);
        }

        // PUT api/leads/lead/5
        public virtual HttpResponseMessage PutContent(int id, TContentDtoType contentDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var oldContent = _contentManager.Get<TContentType>(id);
            if (oldContent == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            contentDto.UpdateEntity(oldContent);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/leads/lead
        public virtual HttpResponseMessage PostContent(TContentDtoType contentDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var content = Services.ContentManager.New<TContentType>(typeof(TContentType).Name);
            contentDto.UpdateEntity(content);
            _contentManager.Create(content);
            contentDto.ContentId = content.Id;

            return Request.CreateResponse(HttpStatusCode.Created, contentDto);
        }

        // DELETE api/leads/lead/5
        public virtual HttpResponseMessage DeleteContent(int id)
        {
            var record = _contentManager.Get(id);
            if (record == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            _contentManager.Remove(record);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private TContentDtoType ConstructDto(TContentType record)
        {
            Type dtoType = typeof(TContentDtoType);
            var constructors = dtoType.GetConstructors();
            foreach (var constructorInfo in constructors)
            {
                var parameters = constructorInfo.GetParameters();
                if (parameters.Count() == 1 &&
                    parameters[0].ParameterType == typeof(TContentType))
                {
                    object[] ps = new object[] { record };
                    object contentDto = constructorInfo.Invoke(ps);
                    return (TContentDtoType)contentDto;
                }
            }
            throw new Exception(dtoType.FullName + " must define a constructor with parameter " + record.GetType().FullName);
        }
    }
}
