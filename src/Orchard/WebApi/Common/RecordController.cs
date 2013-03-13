using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Orchard.Data;

namespace Orchard.WebApi.Common
{
    public abstract class RecordController<TRecordType, TRecordDtoType> : ApiController where TRecordType : class 
        where TRecordDtoType : IDto<TRecordType> 
       
    {
        protected IRepository<TRecordType> _recordRepository;

        public RecordController(IRepository<TRecordType> recordRepository)
        {
            _recordRepository = recordRepository;
        }

        // GET api/leads/lead
        protected IEnumerable<TRecordDtoType> GetRecords()
        {
            var reDtos = new List<TRecordDtoType>();
            var re = _recordRepository.Table.ToList();
            reDtos.AddRange(re.Select(record => this.ConstructDto(record)));
            return reDtos;
        }

        // GET api/leads/lead/5
        protected TRecordDtoType GetRecord(int id)
        {
            var record = _recordRepository.Get(id);
            if (record == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return this.ConstructDto(record);
        }

        // PUT api/leads/lead/5
        protected HttpResponseMessage PutRecord(int id, TRecordDtoType recordDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var oldRecord = _recordRepository.Get(id);
            if (oldRecord == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var newRecord = recordDto.ToEntity();
            _recordRepository.Copy(newRecord, oldRecord);

            try
            {
                _recordRepository.Flush();
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/leads/lead
        protected HttpResponseMessage PostRecord(TRecordDtoType recordDtoDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }


            var record = recordDtoDto.ToEntity();
            _recordRepository.Create(record);
            _recordRepository.Flush();
            recordDtoDto.RecordId = this.GetRecordId(record);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, recordDtoDto);
            //response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = leadDto.LeadId }));
            return response;
        }

        // DELETE api/leads/lead/5
        protected HttpResponseMessage DeleteRecord(int id)
        {
            var record = _recordRepository.Get(id);
            if (record == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }


            var leadDto = this.ConstructDto(record);
            _recordRepository.Delete(record);

            try
            {
                _recordRepository.Flush();
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, leadDto);
        }

        private TRecordDtoType ConstructDto(TRecordType record)
        {
            Type dtoType = typeof(TRecordDtoType);
            var constructors = dtoType.GetConstructors();
            foreach (var constructorInfo in constructors)
            {
                var parameters = constructorInfo.GetParameters();
                if (parameters.Count() == 1 &&
                    parameters[0].ParameterType == typeof(TRecordType))
                {
                    object[] ps = new object[]{record};
                    object recordDto = constructorInfo.Invoke(ps);
                    return (TRecordDtoType) recordDto;
                }
            }
            throw  new Exception(dtoType.FullName + " must define a constructor with parameter " + record.GetType().FullName);
        }

        private object GetRecordId(TRecordType record)
        {
            PropertyInfo idProperty = record.GetType().GetProperty("Id");
            if (idProperty == null)
            {
                throw new Exception(" RecordType must define a property with name 'Id'");
            }
            object idValue = idProperty.GetValue(record,null);
            return idValue;
        }

        private void SetRecordId(TRecordType record, object idValue)
        {
            PropertyInfo idProperty = record.GetType().GetProperty("Id");
            if (idProperty == null)
            {
                throw new Exception(" RecordType must define a property with name 'Id'");
            }
            idProperty.SetValue(record, idValue,null);
        }
    }
}
