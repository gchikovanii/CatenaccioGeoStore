namespace CatenaccioStore.Core.Entities
{
    public class CustomerBasket
    {
        public string Id { get; set; }
        public List<BaksetItem> BaksetItems { get; set; } = new List<BaksetItem>();
        public CustomerBasket()
        {
        }
        public CustomerBasket(string id)
        {
            Id = id;
        }

    }
}
