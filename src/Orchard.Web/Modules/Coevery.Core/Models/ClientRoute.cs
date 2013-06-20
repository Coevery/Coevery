using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Coevery.Core.Models
{
    public class Definition
    {
        public string Url { get; set; }
        public string TemplateUrl { get; set; }
        public string Controller { get; set; }
        public string[] Dependencies { get; set; }
    }

    public class ClientRouteInfo {
        public bool z { get; set; }
        public Definition Definition { get; set; }
        public Dictionary<string, ClientRouteInfo> Children { get; set; }
    }

    public class ClientRoute
    {
        public string StateName { get; set; }
        public ClientRouteInfo ClientRouteInfo { get; set; }
    }


}