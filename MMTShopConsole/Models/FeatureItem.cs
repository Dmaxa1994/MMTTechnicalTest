using Newtonsoft.Json;

namespace MMTShopConsole.Models
{
    class FeatureItem
    {
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("Item_Category_Name")]
        public string FeaturedItemName { get; set; }
    }
}
