using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Coevery.Core.Models
{
    public class Definition
    {
        public string url { get; set; }
        public JRaw templateUrl { get; set; }
        public bool @abstract { get; set; }
        public string controller { get; set; }
        public string[] dependencies { get; set; }
    }

    public class ClientRouteInfo {
        //public bool z { get; set; }
        public Definition definition { get; set; }
        public Dictionary<string, ClientRouteInfo> children { get; set; }
        public Dictionary<string, Definition> Relationships { get; set; }
    }

    public class ClientRoute
    {
        public string StateName { get; set; }
        public ClientRouteInfo ClientRouteInfo { get; set; }
    }


}