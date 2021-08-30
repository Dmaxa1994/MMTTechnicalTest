using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace MMTShopConsole.Web
{
    class HttpHandler
    {
        private string apiPartial = "https://localhost:44325/api/MMTShop/";

        /// <summary>
        /// This API call does the create, update and delete calls
        /// The body is used for create and update, we pass a serialized json string into the body
        /// ID is used for delete and update, nullable as we do not want it for create events as the DB handles the ID
        /// </summary>
        /// <param name="requestType">Is it a delete or Post, so we know what method to call</param>
        /// <param name="aPIAction">Used for routing, api only accepts Create, Get, Update, Delete</param>
        /// <param name="itemType">Used for routing, api only accepts StoreItem, ItemCategories and FeatureItem</param>
        /// <param name="body">Serialized Json string</param>
        /// <param name="id">Obj ID from the database</param>
        /// <returns></returns>
        public async Task<bool?> PerformApiCall(HttpRequestType requestType, string aPIAction, string itemType, string body = "", int? id = null)
        {
            Console.Clear();
            Console.WriteLine("Uploading");
            HttpClient client = new HttpClient();
            StringContent content = new StringContent("");

            if(!string.IsNullOrWhiteSpace(body))
                content = new StringContent(body, Encoding.Unicode, "application/json");

            HttpResponseMessage resp;

            switch(requestType)
            {
                case HttpRequestType.Delete:
                     resp = await client.DeleteAsync($"{apiPartial}{aPIAction}/{itemType}?id={id}");
                    break;
                case HttpRequestType.Post:
                    if(id == null)
                        resp = await client.PostAsync($"{apiPartial}{aPIAction}/{itemType}", content);
                    else
                        resp = await client.PostAsync($"{apiPartial}{aPIAction}/{itemType}?id={id}", content);
                    break;
                default:
                    // we shouldn't get here anyway
                    return null;
            }
            

            Console.Clear();
            Console.WriteLine("Http Response : " + resp.StatusCode);
            try
            {
                return resp.IsSuccessStatusCode;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception in HTTP Request " + ex.Message);
                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
                return null;
            }
        }

        /// <summary>
        /// Performs the GetAsync call
        /// Builds the api URL by using passed in parameters, but it works without passing any in
        /// </summary>
        /// <param name="itemType">Used for routing, api only accepts StoreItem, ItemCategories and FeatureItem</param>
        /// <param name="parameterNames">Parameters names to be added to the url</param>
        /// <param name="parameterValues">parameter values to be passed into the url</param>
        /// <returns></returns>
        public async Task<string> PerformGetApiCall(string itemType, string[] parameterNames = null, string[] parameterValues = null)
        {
            Console.Clear();
            Console.WriteLine("Retrieving");
            HttpClient client = new HttpClient();
            string json = "";

            string api = $"{apiPartial}Get/{itemType}";

            HttpResponseMessage resp;

            //Loop through both parameter names and values, these need to be on a 1-1
            if(parameterValues != null && parameterNames != null)
            {
                if (parameterValues.Length != parameterNames.Length)
                {
                    Console.WriteLine("Parameters mismatch, unable to complete parameteried query, returning full list");
                }
                else
                {
                    for (int i = 0; i < parameterNames.Length; i++)
                    {
                        if (i == 0)
                        {
                            api += $"?{parameterNames[i]}={parameterValues[i]}";
                        }
                        else
                        {
                            api += $"&{parameterNames[i]}={parameterValues[i]}";
                        }
                    }
                }
            }

            try
            {
                resp = await client.GetAsync(api);

                Console.Clear();
                Console.WriteLine("Http Response : " + resp.StatusCode);

                if (resp.IsSuccessStatusCode)
                {
                    json = resp.Content.ReadAsStringAsync().Result.ToString();
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Exception in HTTP Request " + ex.Message);
                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
                return null;
            }

            return json;
        }

        public enum HttpRequestType
        {
            Get,
            Post,
            Delete
        }
    }
}
