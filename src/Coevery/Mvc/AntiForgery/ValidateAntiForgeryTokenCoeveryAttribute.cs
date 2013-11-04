using System;
using System.Web.Mvc;

namespace Coevery.Mvc.AntiForgery {
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateAntiForgeryTokenCoeveryAttribute : FilterAttribute {
    }
}