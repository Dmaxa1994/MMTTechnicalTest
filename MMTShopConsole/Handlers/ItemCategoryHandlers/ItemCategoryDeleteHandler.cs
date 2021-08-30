using MMTShopConsole.Interaction;
using MMTShopConsole.Models;
using MMTShopConsole.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MMTShopConsole.Handlers.ItemCategoryHandlers
{
    class ItemCategoryDeleteHandler
    {
        public ItemCategoryDeleteHandler()
        {
            LoadDeleteItemCategoryDisplay();
        }

        /// <summary>
        /// returns list of item categories for the user to select
        /// </summary>
        private void LoadDeleteItemCategoryDisplay()
        {
            List<ItemCategory> itemCategories = new List<ItemCategory>();

            List<Option> DeleteQueryOptions = new List<Option>();

            HttpHandler handler = new HttpHandler();
            string jsonResponse = handler.PerformGetApiCall("ItemCategories").Result;

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                Console.Clear();
                Console.WriteLine("No Items to display");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                return;
            }

            itemCategories = JsonConvert.DeserializeObject<List<ItemCategory>>(jsonResponse);

            foreach (ItemCategory ic in itemCategories)
            {
                DeleteQueryOptions.Add(new Option(ic.CategoryName, () => DeleteItemCategory(ic)));
            }

            DeleteQueryOptions.Add(new Option("Back", null));

            MenuHandler.menuInstance.HandleUserInput(DeleteQueryOptions, "Select Item To delete");
        }

        /// <summary>
        /// performs the API call for Delete
        /// </summary>
        /// <param name="ic">uses the category name to confirm with user and the ID to delete the entry</param>
        private void DeleteItemCategory(ItemCategory ic)
        {
            Console.Clear();
            Console.WriteLine($"Are you sure you wish to delete {ic.CategoryName} from Item Categories?");
            Console.WriteLine($"y to confirm, any other entry will result in operation aborted");
            string userResponse = Console.ReadLine().ToLower();

            if (userResponse == "y")
            {
                HttpHandler handler = new HttpHandler();
                bool? success = handler.PerformApiCall(HttpHandler.HttpRequestType.Delete, "Delete", "ItemCategories", id: ic.ID).Result;

                if (success.HasValue && success.Value)
                {
                    Console.WriteLine("Successfully deleted");
                    MenuHandler.menuInstance.DeleteEntry = true;
                }
                else
                {
                    Console.WriteLine("Failed to delete");
                    Console.WriteLine(ic.CategoryName);
                }
                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
            }
        }
    }
}
