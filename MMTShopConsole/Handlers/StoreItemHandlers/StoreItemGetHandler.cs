using MMTShopConsole.Web;
using System;
using System.Collections.Generic;

namespace MMTShopConsole.Handlers.StoreItemHandlers
{
    class StoreItemGetHandler
    {
        public StoreItemGetHandler()
        {
            GetAllStoreItems();
        }

        /// <summary>
        /// Returns the store items from the api
        /// Allows the user to enter parameters to filter the results
        /// </summary>
        private void GetAllStoreItems()
        {
            Console.Clear();

            HttpHandler handler = new HttpHandler();
            List<string> ParameterNames = new List<string>();
            List<string> ParameterValues = new List<string>();

            Console.WriteLine("Enter Product Code (SKU), this can be left blank");
            string productCode = Console.ReadLine();

            //no need to ask the user for every possible param if they already supplied an answer
            //this will be changed when the api is configured to accept multiple parameters
            if (string.IsNullOrWhiteSpace(productCode))
            {
                Console.WriteLine("Enter Product Name, this can be left blank");
                string productName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(productName))
                {
                    Console.WriteLine("Enter Product Category Name, this can be left blank");
                    string categoryName = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(categoryName))
                    {
                        ParameterNames.Add("categoryName");
                        ParameterValues.Add(categoryName);
                    }
                }
                else
                {
                    ParameterNames.Add("productName");
                    ParameterValues.Add(productName);
                }
            }
            else
            {
                ParameterNames.Add("productCode");
                ParameterValues.Add(productCode);
            }

            Console.WriteLine("Do you want to filter by featured products?");
            Console.WriteLine("Any response other than y will be taken to mean no");
            string filterByFeatured = Console.ReadLine().ToLower();

            if (filterByFeatured == "y")
            {
                ParameterNames.Add("featuredItem");
                ParameterValues.Add("true");
            }

            string jsonResponse = handler.PerformGetApiCall("StoreItem", ParameterNames.ToArray(), ParameterValues.ToArray()).Result;

            Console.Clear();

            Console.WriteLine(jsonResponse);
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }

    }
}
