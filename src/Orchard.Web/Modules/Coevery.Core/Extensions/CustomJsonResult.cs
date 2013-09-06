using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Coevery.Core
{
    public class CustomJsonResult : JsonResult, ICustomResult
    {
        #region Implementation of ICustomResult

        public bool Success { get; set; }

        public string Message { get; set; }

        public int Value { get; set; }

        public HttpStatusCodeResult HttpStatus { get; set; }

        public void Succeed()
        {
            this.Success = true;
            this.HttpStatus = new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public void Fail(string message)
        {
            this.Success = false;
            this.Message = message;
        }

        public void Try(Action action)
        {
            try
            {
                action();
                this.Succeed();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                this.Fail(ex.Message);
            }
        }

        #endregion

        public CustomJsonResult(bool allowGet = false)
        {
            if (allowGet)
            {
                this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }
            else
            {
                this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var isHttpGet = context.HttpContext.Request.HttpMethod.Equals("get");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && isHttpGet)
            {
                const string error = @"This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.";
                throw new InvalidOperationException(error);
            }

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            response.Write(Serializer.ToJson(this.ToCustomResult()));
        }

        protected virtual ICustomResult ToCustomResult()
        {
            var result = new CustomResult();
            result.Success = this.Success;
            result.Message = this.Message;
            result.Value = this.Value;
            return result;
        }
    }
}
