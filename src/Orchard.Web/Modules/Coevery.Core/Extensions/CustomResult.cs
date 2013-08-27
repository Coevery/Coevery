using System;
using System.Web.Mvc;

namespace Coevery.Core
{
    public class CustomResult : ICustomResult {
        #region Implementation of ICustomResult

        public bool Success { get; set; }

        public string Message { get; set; }

        public int Value { get; set; }

        public void Succeed() {
            this.Success = true;
        }

        public void Fail(string message) {
            this.Success = false;
            this.Message = message;
        }

        public void Try(Action action) {
            try {
                action();
                this.Succeed();
            }
            catch (Exception ex) {
                this.Fail(ex.Message);
            }
        }

        #endregion
    }
}
