using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MMTStoreAPI.Contexts;
using MMTStoreAPI.Models;
using MMTStoreAPI.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace MMTStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MMTShopController : ControllerBase
    {
        private readonly MMTShopContext _context;
        private readonly ILogger<MMTShopController> _logger;

        public MMTShopController(MMTShopContext context,
            ILogger<MMTShopController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region HttpGet
        /// <summary>
        /// Endpoint to return the store items as JSON
        /// </summary>
        /// <param name="productCode">nullable, the SKU of the product</param>
        /// <param name="productName">Name of the product, this does not have to be the complete name</param>
        /// <param name="categoryName">Filter for category</param>
        /// <param name="featuredItem">nullable, filter for featured items</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/StoreItem")]
        public IActionResult GetStoreItems(int? productCode = null, string productName = "", string categoryName = "", bool? featuredItem = null)
        {
            _logger.LogInformation("GetStoreItems Request");
            List<StoreItem> storeItems;


            //make sure that no more than 1 variable is assigned
            //if it is, return bad request, as we are unable to fulfuil what the user wanted
            //This is due to the fact that the queries
            //are not set up for multiple parameters, featuredItem exluded as that is not done
            //via query.
            if ((productCode != null && !string.IsNullOrWhiteSpace(productName)) ||
                (productCode != null && !string.IsNullOrWhiteSpace(categoryName)) ||
                (!string.IsNullOrWhiteSpace(categoryName) && !string.IsNullOrWhiteSpace(productName)))
            {
                _logger.LogInformation("end of request : result 400");
                return BadRequest();
            }

            string dBQuery = "";
            string[] parameterNames = null;
            Object[] parameterValues = null;
            
            //set the relevant parameters based on what variable was passed in
            //if none were then we get the whole list
            //additionally, if we want the featured and category filtered list, we return the whole list from the db
            if (productCode != null)
            {
                dBQuery = DatabaseQueries.GetQueries[DatabaseQueries.GetEnums.GetItemsBySKU];
                parameterNames = new[] { "@SKU" };
                parameterValues = new[] { (Object)productCode.Value };
            }
            else if (!string.IsNullOrEmpty(productName))
            {
                dBQuery = DatabaseQueries.GetQueries[DatabaseQueries.GetEnums.GetItemsByItemName];
                parameterNames = new[] { "@itemName" };
                //modify the value to suit the like command
                parameterValues = new[] { "%"+(Object)productName+"%" };
            }
            else
            {
                dBQuery = DatabaseQueries.GetQueries[DatabaseQueries.GetEnums.GetAllItems];
            }

            try
            {
                storeItems = _context.GetStoreItemsFromDB(dBQuery, parameterNames, parameterValues);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError("Error with sql connection/query {sqlEx}", sqlEx.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unspecified Error with db query {ex}", ex.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }

            //filter by category
            if (!string.IsNullOrEmpty(categoryName))
            {
                storeItems = storeItems.Where(x => x.Category.ToLower() == categoryName.ToLower()).ToList();
            }

            if(featuredItem.HasValue && featuredItem.Value)
            {
                int statusCode = FilterStoreItems(ref storeItems);
                if(statusCode != 200)
                {
                    return StatusCode(statusCode);
                }
            }

            if (storeItems.Count == 0)
            {
                _logger.LogInformation("end of request : result 204");
                return NoContent();
            }

            string output = JsonConvert.SerializeObject(storeItems, Formatting.Indented);

            _logger.LogInformation("end of request : result 200");
            return Ok(output);
        }

        /// <summary>
        /// Filters store items on featured items
        /// returns the status code
        /// takes storeitems by ref so we can modify the list
        /// </summary>
        /// <param name="storeItems">list of store items retrieved from database</param>
        /// <returns></returns>
        public int FilterStoreItems(ref List<StoreItem> storeItems)
        {
            try
            {
                //get the featured items from the database
                List<FeatureItem> featureItems = _context.GetFeaturedItemsFromDB();

                //using temp variables.
                //itemsTemp1 will be the new value of storeItems and is made up of all the featured categories
                //temp 2 is the individual featured cateogry
                List<StoreItem> itemsTmp1 = new List<StoreItem>();
                foreach (FeatureItem item in featureItems)
                {
                    List<StoreItem> itemsTmp2 = new List<StoreItem>();
                    itemsTmp2 = storeItems.Where(x => x.Category == item.FeaturedItemName).ToList();

                    itemsTmp1.AddRange(itemsTmp2);
                }
                storeItems = new List<StoreItem>();
                storeItems.AddRange(itemsTmp1);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError("Error with sql connection/query {sqlEx}", sqlEx.Message);
                _logger.LogInformation("end of request : result 500");
                return 500;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unspecified Error with db query {ex}", ex.Message);
                _logger.LogInformation("end of request : result 500");
                return 500;
            }

            return Ok().StatusCode;
        }

        /// <summary>
        /// return all categories from DB in JSON
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/ItemCategories")]
        public IActionResult GetItemCategories()
        {
            _logger.LogInformation("GetItemCategories Request");
            List<ItemCategory> itemCategories = new List<ItemCategory>();

            try
            {
                itemCategories = _context.GetCategoriesFromDB(DatabaseQueries.GetQueries[DatabaseQueries.GetEnums.GetAllCategories]);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError("Error with sql connection/query {sqlEx}", sqlEx.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unspecified Error with db query {ex}", ex.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }

            string output = JsonConvert.SerializeObject(itemCategories, Formatting.Indented);

            if (itemCategories.Count == 0)
            {
                _logger.LogInformation("end of request : result 204");
                return NoContent();
            }

            _logger.LogInformation("end of request : result 200");
            return Ok(output);
        }

        /// <summary>
        /// return all featured items using the category names in JSON
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/FeatureItem")]
        public IActionResult GetFeatureItem()
        {
            _logger.LogInformation("GetFeatureItem Request");

            List<FeatureItem> featuredItems = new List<FeatureItem>();

            try
            {
                featuredItems = _context.GetFeaturedItemsFromDB();
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError("Error with sql connection/query {sqlEx}", sqlEx.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unspecified Error with db query {ex}", ex.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }

            string output = JsonConvert.SerializeObject(featuredItems, Formatting.Indented);

            if (featuredItems.Count == 0)
            {
                _logger.LogInformation("end of request : result 204");
                return NoContent();
            }

            _logger.LogInformation("end of request : result 200");
            return Ok(output);

        }
        #endregion

        #region HTTPPost:Create
        /// <summary>
        /// Adds new item category to the db
        /// </summary>
        /// <param name="itemCategory">New item for db</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create/ItemCategories")]
        public IActionResult CreateItemCategories([FromBody] ItemCategory itemCategory = null)
        {
            _logger.LogInformation("UpdateItemCategories Request");

            if (itemCategory == null)
            {
                _logger.LogError("Invalid parameter in request");
                _logger.LogInformation("end of request : result 400");
                return BadRequest();
            }

            int? updatedRows = 0;

            string dBQuery = DatabaseQueries.CreateQueries[DatabaseQueries.CreateEnums.CreateItemCategory];
            string[] parameterNames = new[] { "@categoryName", "@categoryFilter" };
            Object[] parameterValues = new[] { itemCategory.CategoryName, itemCategory.CategoryFilter };

            try
            {
                updatedRows += _context.UpdateDB(dBQuery, parameterNames, parameterValues);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError("Error with sql connection/query {sqlEx}", sqlEx.Message);
                if (sqlEx.Number == 2627)
                {
                    _logger.LogInformation("end of request : result 409");
                    return StatusCode(409);
                }
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unspecified Error with db query {ex}", ex.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }

            string output = $"Inserted {updatedRows} Row(s)";

            _logger.LogInformation("end of request : result 200");
            return Ok(output);
        }

        /// <summary>
        /// Adds new store item to the database
        /// </summary>
        /// <param name="storeItem">new item for the db</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create/StoreItem")]
        public IActionResult CreateStoreItem([FromBody] StoreItem storeItem = null)
        {
            _logger.LogInformation("UpdateItemCategories Request");

            if (storeItem == null)
            {
                _logger.LogError("Invalid parameter in request");
                _logger.LogInformation("end of request : result 400");
                return BadRequest();
            }

            int? updatedRows = 0;

            string dBQuery = DatabaseQueries.CreateQueries[DatabaseQueries.CreateEnums.CreateStoreItem];
            string[] parameterNames = new[] { "@SKU", "@name", "@desc", @"price" };
            Object[] parameterValues = new[] { storeItem.SKU.ToString(), storeItem.Name, storeItem.Description, storeItem.Price.ToString() };

            try
            {
                updatedRows += _context.UpdateDB(dBQuery, parameterNames, parameterValues);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError("Error with sql connection/query {sqlEx}", sqlEx.Message);
                if (sqlEx.Number == 2627)
                {
                    _logger.LogInformation("end of request : result 409");
                    return StatusCode(409);
                }
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unspecified Error with db query {ex}", ex.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }

            string output = $"Inserted {updatedRows} Row(s)";

            _logger.LogInformation("end of request : result 200");
            return Ok(output);
        }

        /// <summary>
        /// Gets the featured items from the DB
        /// At the same time, gets the readable name from the item categories
        /// returns JSON of ID from requested feature and Name from item categories
        /// </summary>
        /// <param name="requestedFeaturedItem"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create/FeatureItem")]
        public IActionResult CreateFeatureItem([FromBody] string requestedFeaturedItem = null)
        {
            _logger.LogInformation("CreateFeatureItem Request");

            if (requestedFeaturedItem == null)
            {
                _logger.LogError("Invalid parameter in request");
                _logger.LogInformation("end of request : result 400");
                return BadRequest();
            }

            int? updatedRows = 0;

            string dBQuery = DatabaseQueries.GetQueries[DatabaseQueries.GetEnums.GetCategoriesByName];
            string[] parameterNames = new[] { "@categoryName" };
            Object[] parameterValues = new[] { requestedFeaturedItem };

            try
            {
                List<ItemCategory> itemCategories = _context.GetCategoriesFromDB(dBQuery, parameterNames, parameterValues);

                //check against 1 as the name should be unique in the db
                if (itemCategories.Count != 1)
                {
                    _logger.LogError("Category Does not exist");
                    _logger.LogInformation("end of request : result 400");
                    return BadRequest();
                }

                dBQuery = DatabaseQueries.CreateQueries[DatabaseQueries.CreateEnums.CreateFeaturedItem];
                parameterNames = new[] { "@itemCatID" };
                parameterValues = new[] { itemCategories[0].ID.ToString() };

                updatedRows = _context.UpdateDB(dBQuery, parameterNames, parameterValues);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError("Error with sql connection/query {sqlEx}", sqlEx.Message);
                if (sqlEx.Number == 2627)
                {
                    _logger.LogInformation("end of request : result 409");
                    return StatusCode(409);
                }
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unspecified Error with db query {ex}", ex.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }

            string output = $"Inserted {updatedRows} Row(s)";

            _logger.LogInformation("end of request : result 200");
            return Ok(output);
        }
        #endregion

        #region HttpPost:Update
        /// <summary>
        /// update the selected category, this will update both name and filter
        /// Takes in the ID as a parameter and json in the body
        /// </summary>
        /// <param name="id">Target item</param>
        /// <param name="itemCategory">From Body, the new values</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Update/ItemCategories")]
        public IActionResult UpdateItemCategories(int? id, [FromBody] ItemCategory itemCategory = null)
        {
            _logger.LogInformation("UpdateItemCategories Request");

            if (id == null || itemCategory == null)
            {
                _logger.LogError("Invalid parameter in request");
                _logger.LogInformation("end of request : result 400");
                return BadRequest();
            }

            int? updatedRows = 0;

            string dBQuery = DatabaseQueries.UpdateQueries[DatabaseQueries.UpdateEnums.UpdateItemCategoryById];
            string[] parameterNames = new[] { "@categoryName", "@categoryFilter", "@ID" };
            Object[] parameterValues = new[] { itemCategory.CategoryName, itemCategory.CategoryFilter, id.Value.ToString() };

            try
            {
                updatedRows += _context.UpdateDB(dBQuery, parameterNames, parameterValues);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError("Error with sql connection/query {sqlEx}", sqlEx.Message);
                if (sqlEx.Number == 2627)
                {
                    _logger.LogInformation("end of request : result 409");
                    return StatusCode(409);
                }
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unspecified Error with db query {ex}", ex.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }

            string output = $"Inserted {updatedRows} Row(s)";

            if (updatedRows == 0)
            {
                //when using the console, we should not fall into this
                //this is more for using postman and using browser manually entering a url
                _logger.LogInformation("end of request : result 204");
                return NoContent();
            }

            _logger.LogInformation("end of request : result 200");
            return Ok(output);
        }

        /// <summary>
        /// Updates the store item
        /// Reads in Json from the body, turning it into a store item
        /// The ID is explicitly passed as well
        /// All columns except id will be updated with this call
        /// </summary>
        /// <param name="id">Item being updated</param>
        /// <param name="storeItem">New values</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Update/StoreItem")]
        public IActionResult UpdateStoreItem(int? id, [FromBody] StoreItem storeItem = null)
        {
            _logger.LogInformation("UpdateItemCategories Request");

            if (id == null || storeItem == null)
            {
                _logger.LogError("Invalid parameter in request");
                _logger.LogInformation("end of request : result 400");
                return BadRequest();
            }

            int? updatedRows = 0;

            //retrieve the query and populate the parameters with all properties except category and id
            string dBQuery = DatabaseQueries.UpdateQueries[DatabaseQueries.UpdateEnums.UpdateStoreItemById];
            string[] parameterNames = new[] { "@SKU", "@name", "@desc", @"price", "@ID" };
            Object[] parameterValues = new[] { storeItem.SKU.ToString(), storeItem.Name, storeItem.Description, storeItem.Price.ToString(), id.Value.ToString() };

            try
            {
                updatedRows += _context.UpdateDB(dBQuery, parameterNames, parameterValues);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError("Error with sql connection/query {sqlEx}", sqlEx.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unspecified Error with db query {ex}", ex.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }

            string output = $"Updated {updatedRows} Row(s)";

            if (updatedRows == 0)
            {
                //when using the console, we should not fall into this
                //this is more for using postman and using browser manually entering a url
                _logger.LogInformation("end of request : result 204");
                return NoContent();
            }

            _logger.LogInformation("end of request : result 200");
            return Ok(output);
        }
        #endregion

        #region HttpDelete
        /// <summary>
        /// Performs the delete action
        /// Uses the route to determine where it is deleting from
        /// </summary>
        /// <param name="id">ID for the item being deleted</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/FeatureItem")]
        [Route("Delete/StoreItem")]
        [Route("Delete/ItemCategories")]
        public IActionResult DeleteFeatureItem(int? id = null)
        {
            var route = Request.Path.Value;

            _logger.LogInformation($"route Request");
            
            if (id == null)
            {
                _logger.LogError("Invalid parameter in request");
                _logger.LogInformation("end of request : result 400");
                return BadRequest();
            }

            int? updatedRows = 0;

            string dBQuery;

            //check the route to see what query we need
            switch (route)
            {
                case "/api/MMTShop/Delete/StoreItem":
                    dBQuery = DatabaseQueries.DeleteQueries[DatabaseQueries.DeleteEnums.DeleteStoreItemByID];
                    break;
                case "/api/MMTShop/Delete/FeatureItem":
                    dBQuery = DatabaseQueries.DeleteQueries[DatabaseQueries.DeleteEnums.DeleteFeaturedItemByID];
                    break;
                case "/api/MMTShop/Delete/ItemCategories":
                    dBQuery = DatabaseQueries.DeleteQueries[DatabaseQueries.DeleteEnums.DeleteItemCategoryByID];
                    break;
                default:
                    _logger.LogError("In default of switch somehow");
                    _logger.LogInformation("end of request : result 404");
                    return NotFound();
            }

            string[] parameterNames = new[] { "@id" };
            Object[] parameterValues = new[] { id.ToString() };

            try
            {
                updatedRows = _context.UpdateDB(dBQuery, parameterNames, parameterValues);
                //when we delete a category, we need to make sure it doesn't then exist as a featured item
                //otherwise we need to manually delete
                if (route == "/api/MMTShop/Delete/ItemCategories")
                {
                    _logger.LogInformation("checking if item category was feature item");

                    //only the query has to change
                    dBQuery = DatabaseQueries.DeleteQueries[DatabaseQueries.DeleteEnums.DeleteFeaturedItemByItemCategoryID];
                    updatedRows = _context.UpdateDB(dBQuery, parameterNames, parameterValues);

                    _logger.LogInformation("check completed");
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError("Error with sql connection/query {sqlEx}", sqlEx.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unspecified Error with db query {ex}", ex.Message);
                _logger.LogInformation("end of request : result 500");
                return StatusCode(500);
            }

            string output = $"Deleted {updatedRows} Row(s) {Environment.NewLine}";

            if (updatedRows == 0)
            {
                //when using the console, we should not fall into this
                //this is more for using postman and using browser manually entering a url
                _logger.LogInformation("end of request : result 204");
                return NoContent();
            }

            _logger.LogInformation("end of request : result 200");
            return Ok(output);
        }
        #endregion
    }
}
