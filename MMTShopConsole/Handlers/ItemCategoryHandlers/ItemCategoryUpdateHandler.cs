using MMTShopConsole.Interaction;
using MMTShopConsole.Models;
using MMTShopConsole.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MMTShopConsole.Handlers.ItemCategoryHandlers
{
    class ItemCategoryUpdateHandler
    {
        public ItemCategoryUpdateHandler()
        {
            UpdateItemCategoryDisplay();
        }

        private void UpdateItemCategoryDisplay()
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
                DeleteQueryOptions.Add(new Option(ic.CategoryName, () => UpdateItemCategory(ic)));
            }

            DeleteQueryOptions.Add(new Option("Back", null));

            MenuHandler.menuInstance.HandleUserInput(DeleteQueryOptions, "Select Item To update");
        }

        private void UpdateItemCategory(ItemCategory ic)
        {
            Console.Clear();
            string userResponse = "";

            ItemCategory icNew = new ItemCategory();

            while (userResponse != "y" && userResponse != "e")
            {
                Console.Clear();

                Console.WriteLine($"Enter Category Name: current {ic.CategoryName}");
                Console.WriteLine($"leave blank to keep the same");
                icNew.CategoryName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(icNew.CategoryName))
                    icNew.CategoryName = ic.CategoryName;

                Console.WriteLine($"Enter Category Filter: current {ic.CategoryFilter}");
                Console.WriteLine($"leave blank to keep the same");
                icNew.CategoryFilter = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(icNew.CategoryFilter))
                    icNew.CategoryFilter = ic.CategoryFilter;

                Console.Clear();

                Console.WriteLine("Confirm y/n");
                Console.WriteLine(icNew.ToString());

                userResponse = Console.ReadLine().ToLower();
            }

            // user didn't want to add an item
            if (userResponse == "e")
                return;

            string json = JsonConvert.SerializeObject(icNew);

            HttpHandler handler = new HttpHandler();
            bool? success = handler.PerformApiCall(HttpHandler.HttpRequestType.Post, "Update", "ItemCategories", json, ic.ID).Result;

            if (success.HasValue && success.Value)
            {
                Console.WriteLine("Successfully Updated");
                MenuHandler.menuInstance.UpdatedEntry = true;
            }
            else
            {
                Console.WriteLine("Failed to update");
                Console.WriteLine(ic.CategoryName);
            }
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
        }
    }
}
