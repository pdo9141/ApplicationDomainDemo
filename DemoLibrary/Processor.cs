using System;
using System.Collections.Generic;

namespace DemoLibrary
{
    public class Processor
    {
        private static readonly Dictionary<int, MyCachedObject> myCache = new Dictionary<int, MyCachedObject>();

        public string ProcessRequest(int id)
        {
            if (!myCache.ContainsKey(id))
            {
                myCache.Add(id, new MyCachedObject());
            }

            int cacheSize = myCache[id].Execute(id);
            return string.Format("This is a response message from id {0} with a cache size of {1} in the AppDomain named: {2}", id, cacheSize, AppDomain.CurrentDomain.FriendlyName);
        }

        public string ProcessRequestThrowEx(int id)
        {
            throw new ApplicationException("Oops an error occurred.");
        }
    }
}
