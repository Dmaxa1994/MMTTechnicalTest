using MMTShopConsole.Models;
using Newtonsoft.Json;
using System;

namespace MMTShopConsole.Handlers.StoreItemHandlers
{
    class StoreItemHandler
    {
        /// <summary>
        /// Present the modifiable data to the user asking for input
        /// </summary>
        /// <param name="userResponse">track if the user is happy with the data or if they wanted to break out</param>
        /// <param name="existingStoreItem">existing store item for update methods</param>
        /// <returns></returns>
        public string GetNewOrUpdatedStoreItemJson(ref string userResponse, StoreItem existingStoreItem = null)
        {
            StoreItem newStoreItem = new StoreItem();

            string json = "";

            while (userResponse != "y" && userResponse != "e")
            {
                //display all modifiable properties
                Console.Clear();

                //this will only be true when updating
                if (existingStoreItem != null)
                    Console.WriteLine("If you do not want a parameter to change, leave it blank");

                Console.WriteLine("Enter Item Name");
                newStoreItem.Name = Console.ReadLine();

                Console.WriteLine("Enter Item Description");
                newStoreItem.Description = Console.ReadLine();

                Console.WriteLine("Enter Item SKU");
                string sku = Console.ReadLine();

                Console.WriteLine("Enter Item price (GBP)");
                string price = Console.ReadLine();

                Console.Clear();

                Console.WriteLine("Confirm y/n");

                if (existingStoreItem != null)
                    newStoreItem = EnsureStoreItemIsPopulated(newStoreItem, existingStoreItem, sku, price);

                json = JsonConvert.SerializeObject(newStoreItem, Formatting.Indented);

                Console.WriteLine(json);

                userResponse = Console.ReadLine().ToLower();
            }

            return json;
        }

        /// <summary>
        /// goes through the modifiable entries in the new store item
        /// if any are blank, take the existing value
        /// </summary>
        /// <param name="newStoreItem"></param>
        /// <param name="existingStoreItem"></param>
        /// <param name="sku"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        private StoreItem EnsureStoreItemIsPopulated(StoreItem newStoreItem, StoreItem existingStoreItem, string sku, string price)
        {
            newStoreItem.Category = existingStoreItem.Category;
            newStoreItem.ID = existingStoreItem.ID;

            if (string.IsNullOrWhiteSpace(newStoreItem.Name))
                newStoreItem.Name = existingStoreItem.Name;

            if (string.IsNullOrWhiteSpace(newStoreItem.Description))
                newStoreItem.Description = existingStoreItem.Description;

            if (string.IsNullOrWhiteSpace(newStoreItem.Name))
                newStoreItem.Name = existingStoreItem.Name;

            if (string.IsNullOrWhiteSpace(sku))
                newStoreItem.SKU = existingStoreItem.SKU;
            else
                newStoreItem.SKU = Convert.ToInt32(sku);

            if (string.IsNullOrWhiteSpace(price))
                newStoreItem.Price = existingStoreItem.Price;
            else
                newStoreItem.Price = float.Parse(price);

            return newStoreItem;
        }
    }
}
