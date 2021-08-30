using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace MMTStoreAPI.Models
{
    public class StoreItem
    {
        public StoreItem() { }
        public StoreItem(int _id, string _name, string _description, int _sku, float _price)
        {
            ID = _id;
            Name = _name;
            Description = _description;
            SKU = _sku;
            Price = _price;
        }

        [Key]
        [JsonProperty("Id")]
        public int ID { get; set; }

        [JsonProperty("Item_Name")]
        public string Name { get; set; }

        [JsonProperty("Item_Description")]
        public string Description { get; set; }

        [JsonProperty("Item_Category")]
        public string Category => GetCategoryName();

        [JsonProperty("Item_SKU")]
        public int SKU { get; set; }

        [JsonProperty("Item_Price")]
        public float Price { get; set; }

        
        //This is obtained through a get request on the catagory items table
        //we do not want this in our json output as it holds no value to the json result
        [NotMapped]
        [JsonIgnore]
        public List<ItemCategory> Categories { get; set; } = new List<ItemCategory>();

        /// <summary>
        /// Category is not contained within the table for store items
        /// Categories are defined by SKU range in a seperate table.
        /// </summary>
        /// <returns></returns>
        private string GetCategoryName()
        {
            string skuStr = SKU.ToString();
            foreach(ItemCategory category in Categories)
            {
                string categoryFilter = category.CategoryFilter.ToLower().Replace("x", ".");
                if (Regex.IsMatch(skuStr, categoryFilter))
                    return category.CategoryName;
            }

            return "Undefined";
        }
    }
}
