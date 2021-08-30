using MMTShopConsole.Web;
using System;

namespace MMTShopConsole.Handlers.ItemCategoryHandlers
{
    class ItemCategoryGetHandler
    {
        public ItemCategoryGetHandler()
        {
            GetAllItemCategory();
        }

        private void GetAllItemCategory()
        {
            HttpHandler handler = new HttpHandler();
            string jsonResponse = handler.PerformGetApiCall("ItemCategories").Result;

            Console.WriteLine(jsonResponse);
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }
    }
}
