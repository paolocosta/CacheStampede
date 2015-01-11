using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;


    
    public abstract class BaseCachedMethod<T>
    {

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

            var cacheObject = HttpContext.Current.Cache.Get(CacheKey) as CacheContainer;

            //Check if the object is virtually expired, that means that we have to insert it in cache again 
            //with a longer virtual expiration and behave as we didn't find it in cache
            if (IsVirtuallyExpired(cacheObject))
            {
                var newExpirationTime = cacheObject.ExpirationTime.AddSeconds(_CacheStampedeExtraExpirationTime);
                
                // Here we don't need to set a different virtual expiration time as this object will
                // read only fot the time necessary to perform the query
                AddDataToCache(cacheObject.Value, newExpirationTime, newExpirationTime);

                expirationTimeChanged = true;
            }

            //Happy case, the object was in cache and not virtually expired
            if (cacheObject != null && !expirationTimeChanged)
            {
                return (T)(cacheObject.Value);
            }

            // We retrieve the actual data and insert into in cache with a virtual expiration shorter than
            // the real expiration
            lock (string.Intern(CacheKey))
            {
                T result = LoadData();
                DateTime now = DateTime.Now;
                AddDataToCache(result, now.AddSeconds(_Expiration), now.AddSeconds(_Expiration).AddSeconds(_CacheStampedeExtraExpirationTime));
                return result;
            }
        }

        private static bool IsVirtuallyExpired(CacheContainer cachedObject)
        {
            return cachedObject != null && cachedObject.ExpirationTime < DateTime.Now;
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
