using CatenaccioStore.API.Errors;

namespace CatenaccioStore.Infrastructure.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; set; }
        public ApiValidationErrorResponse() : base(400)
        {
        }
    }
}
