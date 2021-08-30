using System.Collections.Generic;

namespace MMTStoreAPI.Data
{
    public class DatabaseQueries
    {

        /// <summary>
        /// static containers for all the queries used in the API
        /// Uses enums for Readable names as the key for ease of use
        /// Seperated into CRUD tasks again for ease of use
        /// </summary>

        public static Dictionary<GetEnums, string> GetQueries = new Dictionary<GetEnums, string>()
        {
            { GetEnums.GetAllCategories, "SELECT * FROM tbl_Item_Category" },
            { GetEnums.GetAllItems, "SELECT * FROM tbl_Store_Items" },
            { GetEnums.GetItemsBySKU, "SELECT * FROM tbl_Store_Items WHERE tbl_Store_Items.Item_SKU = @SKU" },
            { GetEnums.GetItemsByItemName, "SELECT * FROM tbl_Store_Items WHERE tbl_Store_Items.Item_Name LIKE @itemName" },
            { GetEnums.GetFeaturedItems, "SELECT tbl_Featured_Items.ID, tbl_Featured_Items.Item_Category_ID, tbl_Item_Category.Category_Name FROM tbl_Featured_Items" +
                " INNER JOIN tbl_Item_Category ON tbl_Featured_Items.Item_Category_ID=tbl_Item_Category.Id" },
            { GetEnums.GetCategoriesByName, "SELECT * FROM tbl_Item_Category WHERE tbl_Item_Category.Category_Name = @categoryName" }
        };

        public enum GetEnums
        {
            GetAllCategories,
            GetAllItems,
            GetItemsBySKU,
            GetItemsByItemName,
            GetFeaturedItems,
            GetCategoriesByName,
        }

        public static Dictionary<UpdateEnums, string> UpdateQueries = new Dictionary<UpdateEnums, string>()
        {
            { UpdateEnums.UpdateItemCategoryById, "UPDATE tbl_Item_Category SET Category_Name = @categoryName, Category_Filter = @categoryFilter WHERE Id = @ID" },
            { UpdateEnums.UpdateStoreItemById, "UPDATE tbl_Store_Items SET Item_SKU = @SKU, Item_Name = @name, Item_Description = @desc, Item_Price = @price WHERE Id = @ID" },
        };

        public enum UpdateEnums
        {
            UpdateItemCategoryById,
            UpdateStoreItemById
        }

        public static Dictionary<CreateEnums, string> CreateQueries = new Dictionary<CreateEnums, string>()
        {
            { CreateEnums.CreateItemCategory, "INSERT INTO tbl_Item_Category(Category_Name, Category_Filter) values (@categoryName, @categoryFilter)" },
            { CreateEnums.CreateStoreItem, "INSERT INTO tbl_Store_Items(Item_SKU, Item_Name, Item_Description, Item_Price) values(@SKU, @name, @desc, @price)" },
            { CreateEnums.CreateFeaturedItem, "INSERT INTO tbl_Featured_Items(Item_Category_ID) values(@itemCatID)" },
        };

        public enum CreateEnums
        {
            CreateItemCategory,
            CreateStoreItem,
            CreateFeaturedItem,
        }

        public static Dictionary<DeleteEnums, string> DeleteQueries = new Dictionary<DeleteEnums, string>()
        { 
            { DeleteEnums.DeleteItemCategoryByID, "DELETE FROM tbl_Item_Category WHERE id = @ID" },
            { DeleteEnums.DeleteStoreItemByID, "DELETE FROM tbl_Store_Items WHERE id = @ID" },
            { DeleteEnums.DeleteFeaturedItemByID, "DELETE FROM tbl_Featured_Items WHERE id = @ID" },
            { DeleteEnums.DeleteFeaturedItemByItemCategoryID, "DELETE FROM tbl_Featured_Items WHERE Item_Category_ID = @ID" },
        };

        public enum DeleteEnums
        {
            DeleteItemCategoryByID,
            DeleteStoreItemByID,
            DeleteFeaturedItemByID,
            DeleteFeaturedItemByItemCategoryID
        }
    }
}
