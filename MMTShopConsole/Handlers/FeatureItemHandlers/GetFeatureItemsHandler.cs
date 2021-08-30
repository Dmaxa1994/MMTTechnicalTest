using MMTShopConsole.Web;
using System;

namespace MMTShopConsole.Handlers.FeatureItemHandlers
{
    class GetFeatureItemsHandler
    {
        public GetFeatureItemsHandler()
        {
            GetAllFeatureItems();
        }

        /// <summary>
        /// Returns all items from the Get/FeatureItem endpoint
        /// Displays as Json
        /// </summary>
        private void GetAllFeatureItems()
        {
            HttpHandler handler = new HttpHandler();
            string jsonResponse = handler.PerformGetApiCall("FeatureItem").Result;

            Console.WriteLine(jsonResponse);
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }
    }
}
