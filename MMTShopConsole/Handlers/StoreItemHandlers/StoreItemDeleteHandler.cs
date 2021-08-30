using MMTShopConsole.Interaction;
using MMTShopConsole.Models;
using MMTShopConsole.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MMTShopConsole.Handlers.StoreItemHandlers
{
    class StoreItemDeleteHandler
    {
        public StoreItemDeleteHandler()
        {
            DeleteStoreItemDisplay();
        }

        /// <summary>
        /// Get all store items from the api
        /// display them as json to the user
        /// the selected item will be up for deletion
        /// </summary>
        private void DeleteStoreItemDisplay()
        {
            List<StoreItem> storeItems = new List<StoreItem>();

            List<Option> UpdateQueryOptions = new List<Option>();

            HttpHandler handler = new HttpHandler();
            string jsonResponse = handler.PerformGetApiCall("StoreItem").Result;

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                Console.Clear();
                Console.WriteLine("No Items to display");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                return;
            }

            storeItems = JsonConvert.DeserializeObject<List<StoreItem>>(jsonResponse);

            foreach (StoreItem si in storeItems)
            {
                string json = JsonConvert.SerializeObject(si, Formatting.Indented);
                UpdateQueryOptions.Add(new Option(json, () => DeleteStoreItem(si)));
            }

            UpdateQueryOptions.Add(new Option("Back", null));

            MenuHandler.menuInstance.HandleUserInput(UpdateQueryOptions, "Select Item To Delete");
        }

        /// <summary>
        /// Gets final confirmation from user they want to delete the item
        /// if yes, perform the api call
        /// </summary>
        /// <param name="storeItem">item to delete</param>
        private void DeleteStoreItem(StoreItem storeItem)
        {
            Console.Clear();
            Console.WriteLine($"Are you sure you wish to delete {storeItem.Name}?");
            Console.WriteLine($"y to confirm, any other entry will result in operation aborted");
            string userResponse = Console.ReadLine().ToLower();

            if (userResponse != "y")
                return;

            HttpHandler handler = new HttpHandler();
            bool? success = handler.PerformApiCall(HttpHandler.HttpRequestType.Delete, "Delete", "StoreItem", id: storeItem.ID).Result;

            if (success.HasValue && success.Value)
            {
                Console.WriteLine("Successfully Deleted item");
                MenuHandler.menuInstance.DeleteEntry = true;
            }
            else
            {
                Console.WriteLine("Failed to Delet item");
                Console.WriteLine(storeItem.Name);
            }
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
        }
    }
}
