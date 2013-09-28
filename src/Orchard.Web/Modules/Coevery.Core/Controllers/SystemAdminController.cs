using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coevery.Core.Admin;
using Coevery.Core.ClientRoute;
using Orchard.Themes;

namespace Coevery.Core.Controllers
{
    public class SystemAdminController : Controller
    {
        private readonly IClientRouteTableManager _clientRouteTableManager;
        public SystemAdminController(IClientRouteTableManager clientRouteTableManager)
        {
            _clientRouteTableManager = clientRouteTableManager;
            
        }

        [SystemAdmin, Themed]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            var routes = _clientRouteTableManager.GetRouteTable(false);
            stateScript = "$stateProvider";
            GetStateScript(routes, null);
            stateScript += ";";
            ViewBag.stateScript = stateScript;
            return View();
        }

        private string stateScript = string.Empty;
        private void GetStateScript(dynamic states, string parentstate)
        {
            foreach (KeyValuePair<string, dynamic> item in states)
            {
                foreach (KeyValuePair<string, dynamic> ci1 in item.Value)
                {
                    var currentstate = item.Key;
                    if (parentstate != null)
                    {
                        currentstate = parentstate + "." + currentstate;
                    }
                    if (ci1.Key.ToLower() == "definition")
                    {
                        stateScript += ".state('" + currentstate + "',{";
                        foreach (KeyValuePair<string, dynamic> ci2 in ci1.Value)
                        {
                            switch (ci2.Key.ToLower())
                            {
                                case "abstract":
                                    stateScript += ci2.Key + ":" + ci2.Value.ToString().ToLower() + ",";
                                    break;
                                case "url":
                                case "template":
                                case "controller":
                                    stateScript += ci2.Key + ":'" + ci2.Value.ToString() + "',";
                                    break;
                                case "templateurl":
                                case "templateprovider":
                                    stateScript += ci2.Key + ":" + ci2.Value.ToString() + ",";
                                    break;
                                case "views":
                                    stateScript += "views:{";
                                    foreach (KeyValuePair<string, dynamic> ci3 in ci2.Value)
                                    {
                                        stateScript += "'" + ci3.Key + "':{";
                                        foreach (KeyValuePair<string, dynamic> ci4 in ci3.Value)
                                        {
                                            switch (ci4.Key.ToLower())
                                            {
                                                case "template":
                                                case "controller":
                                                    stateScript += ci4.Key + ":" + "'" + ci4.Value + "',";
                                                    break;
                                                case "templateurl":
                                                case "templateprovider":
                                                    stateScript += ci4.Key + ":" + ci4.Value + ",";
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        stateScript += "},";
                                    }
                                    stateScript += "},";
                                    break;
                                case "dependencies":
                                    stateScript += "resolve:{";
                                    var count = 0;
                                    foreach (string dependency in ci2.Value)
                                    {
                                        count++;
                                        stateScript += "resolveVal_" + count + ":function($q, $rootScope){";
                                        stateScript += "var defer = $q.defer();";
                                        stateScript += "require(['" + dependency + "'], function(){";
                                        stateScript += "defer.resolve();";
                                        stateScript += "$rootScope.$apply();";
                                        stateScript += "});";
                                        stateScript += "return defer.promise;";
                                        stateScript += "},";
                                    }
                                    stateScript += "},";
                                    break;
                                default:
                                    break;
                            }
                        }
                        stateScript += "})";
                    }
                    else if (ci1.Key.ToLower() == "children")
                    {
                        GetStateScript(ci1.Value, currentstate);
                    }
                }
            }
        }
    }
}