using MMTShopConsole.Interaction;
using MMTShopConsole.Models;
using MMTShopConsole.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MMTShopConsole.Handlers.StoreItemHandlers
{
    class StoreItemUpdateHandler : StoreItemHandler
    {
        public StoreItemUpdateHandler()
        {
            UpdateStoreItemDisplay();
        }

        /// <summary>
        /// Loops through all store items
        /// creating a new option for each
        /// the user can select which one to update
        /// </summary>
        private void UpdateStoreItemDisplay()
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

            foreach (StoreItem storeItem in storeItems)
            {
                string json = JsonConvert.SerializeObject(storeItem, Formatting.Indented);
                UpdateQueryOptions.Add(new Option(json, () => UpdateStoreItem(storeItem)));
            }

            UpdateQueryOptions.Add(new Option("Back", null));

            MenuHandler.menuInstance.HandleUserInput(UpdateQueryOptions, "Select Item To update");
        }

        /// <summary>
        /// Display the modifiable values to the user and ask for new data
        /// Once done, perform the api call
        /// </summary>
        /// <param name="existingStoreItem">Existing store item, used for variables that have not changed</param>
        private void UpdateStoreItem(StoreItem existingStoreItem)
        {
            Console.Clear();

            string userResponse = "";

            string json = GetNewOrUpdatedStoreItemJson(ref userResponse, existingStoreItem);

            // user didn't want to add an item
            if (userResponse == "e")
                return;

            HttpHandler handler = new HttpHandler();
            bool? success = handler.PerformApiCall(HttpHandler.HttpRequestType.Post, "Update", "StoreItem", json, existingStoreItem.ID).Result;

            if (success.HasValue && success.Value)
            {
                Console.WriteLine("Successfully Updated item");
                MenuHandler.menuInstance.UpdatedEntry = true;
            }
            else
            {
                Console.WriteLine("Failed to Updated item");
                Console.WriteLine(json);
            }
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
        }
    }
}
