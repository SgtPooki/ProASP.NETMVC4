using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;

namespace LanguageFeatures.Models
{
    public class MyAsyncMethods
    {
        public static Task<long?> GetPageLengthOld()
        {
            HttpClient client = new HttpClient();
            var httpTask = client.GetAsync("http://apress.com");

            // other things n stuff can be done here because the above is asynchronous (not blocking)

            return httpTask.ContinueWith((Task<HttpResponseMessage> deferred) =>
            {
                return deferred.Result.Content.Headers.ContentLength;
            });
        }

        public async static Task<long?> GetPageLengthNew()
        {
            HttpClient client = new HttpClient();
            var httpMessage = await client.GetAsync("http://apress.com");

            // other things n stuff can be done here because the above is asynchronous (not blocking)

            return httpMessage.Content.Headers.ContentLength;
        }
    }
}