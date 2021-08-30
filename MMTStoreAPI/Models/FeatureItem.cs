using Newtonsoft.Json;

namespace MMTStoreAPI.Models
{
    public class FeatureItem
    {
        public FeatureItem() { }

        [JsonProperty("ID")]
        public int ID { get; set; }

        //this is returned from the item category table. It is obtained using a join
        [JsonProperty("Item_Category_Name")]
        public string FeaturedItemName { get; set; }
    }
}
