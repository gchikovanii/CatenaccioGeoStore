namespace CatenaccioStore.Core.Entities
{
    public class CustomerBasket
    {
        public string Id { get; set; }
        public List<BaksetItem> BaksetItems { get; set; } = new List<BaksetItem>();
        public int? DeliveryMethodId { get; set; }
        public string? ClientSecret { get; set; }
        public string? PaymentIntentId { get; set; }
        public CustomerBasket()
        {
        }
        public CustomerBasket(string id)
        {
            Id = id;
        }

    }
}
