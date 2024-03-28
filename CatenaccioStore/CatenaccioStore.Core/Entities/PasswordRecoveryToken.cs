namespace CatenaccioStore.Core.Entities
{
    public class PasswordRecoveryToken : BaseEntity
    {
        public string Token { get; set; }
        public string UserEmail { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
