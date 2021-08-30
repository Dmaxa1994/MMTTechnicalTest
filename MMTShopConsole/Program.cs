using System.Collections.Generic;
using MMTShopConsole.Interaction;
using MMTShopConsole.Handlers.StoreItemHandlers;
using MMTShopConsole.Handlers.ItemCategoryHandlers;
using MMTShopConsole.Handlers.FeatureItemHandlers;

namespace MMTShopConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            MenuHandler.menuInstance = new MenuHandler();

            List<Option> queryOptions = new List<Option>()
            {
                new Option("Get Queries", () => LoadGetQueries() ),
                new Option("Create Queries", () => LoadCreateQueries()),
                new Option("Update Queries", () => LoadUpdateQueries() ),
                new Option("Delete Queries", () => LoadDeleteQueries() ),
                new Option("Exit", null ),
            };

            MenuHandler.menuInstance.HandleUserInput(queryOptions);
        }

        private static void LoadCreateQueries()
        {
            List<Option> CreateQueryOptions = new List<Option>()
            {
                new Option("Store Item", () => new StoreItemCreateHandler() ),
                new Option("Item Category", () => new ItemCategoryCreateHandler()),
                new Option("Featured Items", () => new FeatureItemCreateHandler()),
                new Option("Back", null ),
            };

            MenuHandler.menuInstance.HandleUserInput(CreateQueryOptions, "Create Items");
        }

        private static void LoadUpdateQueries()
        {
            List<Option> UpdateQueryOptions = new List<Option>()
            {
                new Option("Store Item", () =>new StoreItemUpdateHandler() ),
                new Option("Item Category", () => new ItemCategoryUpdateHandler()),
                new Option("Back", null ),
            };

            MenuHandler.menuInstance.HandleUserInput(UpdateQueryOptions, "Update Items");
        }

        private static void LoadDeleteQueries()
        {
            List<Option> DeleteQueryOptions = new List<Option>()
            {
                new Option("Store Item", () => new StoreItemDeleteHandler() ),
                new Option("Item Category", () => new ItemCategoryDeleteHandler()),
                new Option("Featured Items", () => new FeatureItemDeleteHandler()),
                new Option("Back", null ),
            };

            MenuHandler.menuInstance.HandleUserInput(DeleteQueryOptions, "Delete Items");
        }

        private static void LoadGetQueries()
        {
            List<Option> GetQueryOptions = new List<Option>()
            {
                new Option("Store Item", () => new StoreItemGetHandler() ),
                new Option("Item Category", () => new ItemCategoryGetHandler()),
                new Option("Featured Items", () => new GetFeatureItemsHandler()),
                new Option("Back", null ),
            };

            MenuHandler.menuInstance.HandleUserInput(GetQueryOptions, "Get Items");
        }
    }
}
