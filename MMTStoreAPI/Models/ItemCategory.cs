using Newtonsoft.Json;

namespace MMTStoreAPI.Models
{
    public class ItemCategory
    {
        public ItemCategory() { }
        public ItemCategory(int _id, string _categoryName, string _categoryFilter)
        {
            ID = _id;
            CategoryName = _categoryName;
            CategoryFilter = _categoryFilter;
        }

        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("Category_Name")]
        public string CategoryName { get; set; }

        [JsonProperty("Category_Filter")]
        public string CategoryFilter { get; set; }
    }
}
