using System;
using System.Collections.Generic;
using System.Linq;

namespace Coevery.Caching {
    public class DefaultCacheContextAccessor : ICacheContextAccessor {
        [ThreadStatic]
        private static IAcquireContext _threadInstance;

        public static IAcquireContext ThreadInstance {
            get { return _threadInstance; }
            set { _threadInstance = value; }
        }

        public IAcquireContext Current {
            get { return ThreadInstance; }
            set { ThreadInstance = value; }
        }
    }
}