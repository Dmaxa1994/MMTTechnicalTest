using MMTShopConsole.Interaction;
using MMTShopConsole.Models;
using MMTShopConsole.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MMTShopConsole.Handlers.FeatureItemHandlers
{
    class FeatureItemCreateHandler
    {
        public FeatureItemCreateHandler()
        {
            LoadCreateFeatureOptionsDisplay();
        }

        /// <summary>
        /// Creates the display for the featured items
        /// A featured item is an ItemCategory that is being promoted
        /// so in the create, we load all the current categories and let the user select which one to promote
        /// </summary>
        private void LoadCreateFeatureOptionsDisplay()
        {
            //get this from the api
            List<ItemCategory> itemCategories = new List<ItemCategory>();

            List<Option> CreateQueryOptions = new List<Option>();

            HttpHandler handler = new HttpHandler();
            string jsonResponse = handler.PerformGetApiCall("ItemCategories").Result;

            itemCategories = JsonConvert.DeserializeObject<List<ItemCategory>>(jsonResponse);

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                Console.Clear();
                Console.WriteLine("No Items to display");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                return;
            }

            foreach (ItemCategory ic in itemCategories)
            {
                CreateQueryOptions.Add(new Option(ic.CategoryName, () => CreateNewFeatureItem(ic)));
            }

            CreateQueryOptions.Add(new Option("Back", null));

            MenuHandler.menuInstance.HandleUserInput(CreateQueryOptions, "Select Item to make a featured product");
        }

        /// <summary>
        /// Perfoms the api call to add the ItemCategory to the promoted list
        /// This does not effect the ItemCategory
        /// </summary>
        /// <param name="ic">The selected Category for promotion</param>
        private void CreateNewFeatureItem(ItemCategory ic)
        {
            Console.Clear();

            string featuredName = ic.CategoryName;

            Console.Clear();
            Console.WriteLine($"Are you sure you wish to make {ic.CategoryName} a featured items?");
            Console.WriteLine($"y to confirm, any other entry will result in operation aborted");
            string userResponse = Console.ReadLine().ToLower();

            if (userResponse == "y")
            {
                string json = JsonConvert.SerializeObject(featuredName);

                HttpHandler handler = new HttpHandler();
                bool? success = handler.PerformApiCall(HttpHandler.HttpRequestType.Post, "Create", "FeatureItem", json).Result;

                if (success.HasValue && success.Value)
                {
                    Console.WriteLine("Successfully Added item");
                }
                else
                {
                    Console.WriteLine("Failed to add item");
                    Console.WriteLine(featuredName);
                }
                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
            }
        }
    }
}
