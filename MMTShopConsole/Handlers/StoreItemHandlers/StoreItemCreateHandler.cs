using MMTShopConsole.Web;
using System;

namespace MMTShopConsole.Handlers.StoreItemHandlers
{
    class StoreItemCreateHandler : StoreItemHandler
    {
        public StoreItemCreateHandler()
        {
            CreateNewStoreItem();
        }

        /// <summary>
        /// Loop through the modifiable properties of the store item class
        /// asking the user to input a value
        /// </summary>
        private void CreateNewStoreItem()
        {
            Console.Clear();

            string userResponse = "";

            string json = GetNewOrUpdatedStoreItemJson(ref userResponse);

            // user didn't want to add an item
            if (userResponse == "e")
                return;

            //get the api handler
            HttpHandler handler = new HttpHandler();
            bool? success = handler.PerformApiCall(HttpHandler.HttpRequestType.Post, "Create", "StoreItem", json).Result;

            if (success.HasValue && success.Value)
            {
                Console.WriteLine("Successfully Added item");
            }
            else
            {
                Console.WriteLine("Failed to add item");
                Console.WriteLine(json);
            }
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
        }
    }
}
