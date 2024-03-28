namespace CatenaccioStore.Core.Entities
{
    public class UserConfirmationToken : BaseEntity
    {
        public string Token { get; set; }
        public string UserEmail { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
