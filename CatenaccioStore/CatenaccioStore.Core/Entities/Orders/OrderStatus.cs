using System.Runtime.Serialization;

namespace CatenaccioStore.Core.Entities.Orders
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "Payment Recevied")]
        PaymentRecevied,
        [EnumMember(Value = "Payment Failed")]
        PaymentFailed,
        [EnumMember(Value = "Product Shipped")]
        Shipped
    }
}
