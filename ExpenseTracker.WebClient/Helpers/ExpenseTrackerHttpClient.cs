using System;
using System.Net.Http;
using System.Net.Http.Headers;
using ExpenseTracker.Constants;

namespace ExpenseTracker.WebClient.Helpers
{
    public class ExpenseTrackerHttpClient
    {
        public static HttpClient GetClient(string requestedVersion = null)
        {
            var client = new HttpClient {BaseAddress = new Uri(ExpenseTrackerConstants.ExpenseTrackerApi)};

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (requestedVersion != null)
            {
                // through a custom request header
                //client.DefaultRequestHeaders.Add("api-version", requestedVersion);

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.expensetrackerapi.v"
                                                        + requestedVersion + "+json"));
            }

            return client;
        }
    }
}