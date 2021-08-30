using Newtonsoft.Json;

namespace MMTShopConsole.Models
{
    class ItemCategory
    {
        public int ID { get; set; }
        [JsonProperty("Category_Name")]
        public string CategoryName { get; set; }
        [JsonProperty("Category_Filter")]
        public string CategoryFilter { get; set; }

        public override string ToString()
        {
            return $"{CategoryName} : {CategoryFilter}";
        }
    }
}
