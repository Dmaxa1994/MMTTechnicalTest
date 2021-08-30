using MMTShopConsole.Interaction;
using MMTShopConsole.Models;
using MMTShopConsole.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MMTShopConsole.Handlers.FeatureItemHandlers
{
    class FeatureItemDeleteHandler
    {
        public FeatureItemDeleteHandler()
        {
            LoadDeleteFeatureOptionsDisplay();
        }

        /// <summary>
        /// Loads all the featured items to the screen, instructing the user to select which on to delete
        /// </summary>
        private void LoadDeleteFeatureOptionsDisplay()
        {
            //get this from the api
            List<FeatureItem> featureItems = new List<FeatureItem>();

            List<Option> DeleteQueryOptions = new List<Option>();

            HttpHandler handler = new HttpHandler();
            string jsonResponse = handler.PerformGetApiCall("FeatureItem").Result;


            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                Console.Clear();
                Console.WriteLine("No Items to display");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                return;
            }

            featureItems = JsonConvert.DeserializeObject<List<FeatureItem>>(jsonResponse);

            foreach (FeatureItem fi in featureItems)
            {
                DeleteQueryOptions.Add(new Option(fi.FeaturedItemName, () => DeleteFeatureItem(fi)));
            }

            DeleteQueryOptions.Add(new Option("Back", null));

            MenuHandler.menuInstance.HandleUserInput(DeleteQueryOptions, "Select Item To delete");
        }

        /// <summary>
        /// Performs the api call to delete the promoted item
        /// This does not effect the item category that the table is referencing
        /// </summary>
        /// <param name="fi">Feature to be deleted</param>
        private void DeleteFeatureItem(FeatureItem fi)
        {
            Console.Clear();
            Console.WriteLine($"Are you sure you wish to delete {fi.FeaturedItemName} from featured items?");
            Console.WriteLine($"y to confirm, any other entry will result in operation aborted");
            string userResponse = Console.ReadLine().ToLower();

            if (userResponse == "y")
            {
                HttpHandler handler = new HttpHandler();
                bool? success = handler.PerformApiCall(HttpHandler.HttpRequestType.Delete, "Delete", "FeatureItem", id: fi.ID).Result;

                if (success.HasValue && success.Value)
                {
                    Console.WriteLine("Successfully deleted");
                    MenuHandler.menuInstance.DeleteEntry = true;
                }
                else
                {
                    Console.WriteLine("Failed to delete");
                    Console.WriteLine(fi.FeaturedItemName);
                }
                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
            }
        }
    }
}
