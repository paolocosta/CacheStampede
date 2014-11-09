﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;


    public class LoadProductMethod : BaseCachedMethod<Product>
    {

        /// <summary>
        /// Complete constructor, of course you can define a simpler one with less choices
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="expiration"></param>
        /// <param name="priority"></param>
        /// <param name="useCache"></param>
        /// <param name="doCallBack"></param>
        public LoadProductMethod(string productId, int expiration, int cacheStampedeExtraExpirationTime, System.Web.Caching.CacheItemPriority priority)
        {
            _productId = productId;
            _Expiration = expiration;
            _CacheStampedeExtraExpirationTime = cacheStampedeExtraExpirationTime;
            _Priority = priority;
           
        }

        /// <summary>
        /// This is the only parameter used by this method
        /// </summary>
        string _productId;

        /// <summary>
        /// This method builds a unique string generated by the parameters set (in this case only one)
        /// </summary>
        /// <returns></returns>
        protected override string GetCacheKey()
        {
            return _productId;
        }

        /// <summary>
        /// This method is a concrete implementation of an abstract method and contains the 
        /// code that retrieves the data from the data source
        /// </summary>
        /// <returns></returns>
        protected override Product LoadData()
        {
            System.Web.HttpContext.Current.Trace.Warn("Product LoadData", CacheKey);
            //This call simulate a long time running query
            System.Threading.Thread.Sleep(5000);
            
            Product product = new Product(_productId);
            return product;
        }
    }
