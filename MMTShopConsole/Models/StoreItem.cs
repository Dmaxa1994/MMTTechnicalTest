using Newtonsoft.Json;

namespace MMTShopConsole.Models
{
    class StoreItem
    {
        public StoreItem() { }

        [JsonProperty("Id")]
        public int ID { get; set; }
        [JsonProperty("Item_Name")]
        public string Name { get; set; }
        [JsonProperty("Item_Description")]
        public string Description { get; set; }
        [JsonProperty("Item_Category")]
        public string Category { get; set; }
        [JsonProperty("Item_SKU")]
        public int SKU { get; set; }
        [JsonProperty("Item_Price")]
        public float Price { get; set; }
    }
}
