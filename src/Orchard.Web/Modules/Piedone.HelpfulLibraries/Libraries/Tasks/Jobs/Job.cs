using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    public interface IJob
    {
        string Industry { get; }
        T Context<T>();
    }

    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Jobs")]
    public class Job : IJob
    {
        private readonly string _contextDefinition;
        private object _context;

        public string Industry { get; private set; }


        public Job(string industry, string contextDefinition)
        {
            Industry = industry;
            _contextDefinition = contextDefinition;
        }


        public T Context<T>()
        {
            if (_context != null) return (T)_context;

            _context = JsonConvert.DeserializeObject<T>(
                            _contextDefinition,
                            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            return (T)_context;
        }
    }
}
