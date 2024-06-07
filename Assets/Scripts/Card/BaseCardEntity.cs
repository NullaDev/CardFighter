using Newtonsoft.Json;

namespace Card
{
    [JsonConverter(typeof(CardJsonConverter))]
    public abstract class BaseCardEntity
    {
        public readonly string id;
        public readonly string name;
        public readonly string description;
        public readonly string imageId;
        public readonly int orgrinCost;

        protected BaseCardEntity(string id, string name, string description, string imageId, int orgrinCost)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.imageId = imageId;
            this.orgrinCost = orgrinCost;
        }


        public abstract CardInstance newInstance();

        public string type()
        {
            return this.GetType().FullName;
        }
    }
}