using MMTShopConsole.Models;
using MMTShopConsole.Web;
using Newtonsoft.Json;
using System;

namespace MMTShopConsole.Handlers.ItemCategoryHandlers
{
    class ItemCategoryCreateHandler
    {
        public ItemCategoryCreateHandler()
        {
            CreateNewItemCategory();
        }

        /// <summary>
        /// Displays the fields for item category to the user and gets the values the user entered
        /// </summary>
        private void CreateNewItemCategory()
        {
            Console.Clear();

            string userResponse = "";

            ItemCategory ic = new ItemCategory();

            while (userResponse != "y")
            {
                Console.Clear();

                Console.WriteLine("Enter Category Name");
                ic.CategoryName = Console.ReadLine();

                Console.WriteLine("Enter Category Filter i.e. 1xxxx");
                ic.CategoryFilter = Console.ReadLine();

                Console.Clear();

                Console.WriteLine("Confirm y/n");
                Console.WriteLine(ic.ToString());

                userResponse = Console.ReadLine().ToLower();
            }

            string json = JsonConvert.SerializeObject(ic);

            HttpHandler handler = new HttpHandler();
            bool? success = handler.PerformApiCall(HttpHandler.HttpRequestType.Post, "Create", "ItemCategories", json).Result;

            if (success.HasValue && success.Value)
            {
                Console.WriteLine("Successfully Added item");
            }
            else
            {
                Console.WriteLine("Failed to add item");
                Console.WriteLine(ic.ToString());
            }
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
        }
    }
}
