using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;


    
    public abstract class BaseCachedMethod<T>
    {
non compila
        internal class CacheContainer
        {
            internal T Value { get; set; }
            internal DateTime ExpirationTime { get; set; }

        }
        /// <summary>
        /// cache expiration in seconds. Default is 60
        /// </summary>
        protected int _Expiration = 60;

        /// <summary>
        /// The increased expiration time in seconds added to the object in order to avoid cache stampede. Default is 10.
        /// </summary>
        protected int _CacheStampedeExtraExpirationTime = 10;

        /// <summary>
        /// Cache priority
        /// </summary>
        protected System.Web.Caching.CacheItemPriority _Priority = System.Web.Caching.CacheItemPriority.Normal;

        

        /// <summary>
        /// This property builds the cache key by using the reflected name of the class and the GetCacheKey 
        /// method implemented in the concrete class
        /// </summary>
        protected string CacheKey
        {
            get
            {
                return this.GetType().ToString() + "-" + this.GetCacheKey();
            }
        }
        
        /// <summary>
        /// This abstract method has to be redefined in the concrete class in order to define a unique cache key
        /// </summary>
        /// <returns></returns>
        protected abstract string GetCacheKey();

        /// <summary>
        /// This abstract method has to be implemented in the concrete class 
        /// and wiil contain the code that performs the query
        /// </summary>
        /// <returns></returns>
        protected abstract T LoadData();

        /// <summary>
        /// Gets the method data from data source or cache
        /// </summary>
        /// <returns></returns>
        public T GetData()
        {
            var expirationTimeChanged = false;
            var cacheObject = GetCachedObject(out expirationTimeChanged);

            Func<bool> isCachedObjectValid = () => cacheObject != null && !expirationTimeChanged;

            if (isCachedObjectValid())
            {
                return (T)(cacheObject.Value);
            }

            lock (string.Intern(CacheKey))
            {
                if (!expirationTimeChanged)
                {
                    cacheObject = GetCachedObject(out expirationTimeChanged);
                    if (isCachedObjectValid())
                    {
                        return (T)(cacheObject.Value);
                    }
                }
                return LoadRealDataAndContextuallyAddToCache();
            }
        }

        /// <summary>
        /// Retrieves the object from cache and immediately ches whether it is virtually expired.
        /// If so, the expiration time is increased and the object is inserted again in cache
        /// </summary>
        /// <param name="expirationTimeChanged"></param>
        /// <returns></returns>
        private CacheContainer GetCachedObject(out bool expirationTimeChanged)
        {
            expirationTimeChanged = false;
            var cachedObject =  HttpContext.Current.Cache.Get(CacheKey) as CacheContainer;
            expirationTimeChanged = IfVirtuallyExpiredIncreaseRealExpirationTimeAndimmediatelyInsertInCache(expirationTimeChanged, cachedObject);
            return cachedObject;
        }

        private bool IfVirtuallyExpiredIncreaseRealExpirationTimeAndimmediatelyInsertInCache(bool expirationTimeChanged, CacheContainer cachedObject)
        {
            if (cachedObject != null && cachedObject.ExpirationTime < DateTime.Now)
            {
                var newExpirationTime = cachedObject.ExpirationTime.AddSeconds(_CacheStampedeExtraExpirationTime);
                AddDataToCache(cachedObject.Value, newExpirationTime, newExpirationTime);
                expirationTimeChanged = true;
            }
            return expirationTimeChanged;
        }

        /// <summary>
        /// Loads the actual data from the data source and inserts the result in cache
        /// </summary>
        /// <returns></returns>
        private T LoadRealDataAndContextuallyAddToCache()
        {
            T result = LoadData();
            DateTime now = DateTime.Now;
            AddDataToCache(result, now.AddSeconds(_Expiration), now.AddSeconds(_Expiration).AddSeconds(_CacheStampedeExtraExpirationTime));
            return result;
        }

        /// <summary>
        /// Adds the data to cache
        /// </summary>
        /// <param name="localResult"></param>
        private void AddDataToCache(T localResult, DateTime virtualExpiration, DateTime realExpiration)
        {
            CacheContainer obj = new CacheContainer()
            {
                Value = localResult,
                ExpirationTime = virtualExpiration
            };
            HttpContext.Current.Cache.Insert(CacheKey, obj, null, realExpiration, System.Web.Caching.Cache.NoSlidingExpiration, _Priority, null);
        }
    }
