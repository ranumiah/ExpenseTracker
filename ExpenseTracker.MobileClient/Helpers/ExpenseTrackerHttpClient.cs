using System;
using System.Net.Http;
using System.Net.Http.Headers;
using ExpenseTracker.Constants;
using Marvin.HttpCache;


namespace ExpenseTracker.MobileClient.Helpers
{

    public static class ExpenseTrackerHttpClient
    {

        private static HttpClient _currentClient = null;

        public static HttpClient GetClient()
        {
            if (_currentClient == null)
            {

                _currentClient = new HttpClient(new HttpCacheHandler {InnerHandler = new HttpClientHandler()});

                _currentClient.BaseAddress = new Uri(ExpenseTrackerConstants.ExpenseTrackerApi);

                _currentClient.DefaultRequestHeaders.Accept.Clear();
                _currentClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
 
            }

            return _currentClient;
        }
         
    }

     
}