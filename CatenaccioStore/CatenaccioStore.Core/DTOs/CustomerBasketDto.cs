using CatenaccioStore.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace CatenaccioStore.Core.DTOs
{
    public class CustomerBasketDto
    {
        [Required]
        public string Id { get; set; }
        public List<BaksetItemDto> BaksetItems { get; set; } 
    }
}
