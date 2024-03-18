using CatenaccioStore.API.Errors;
using Microsoft.AspNetCore.Mvc;

namespace CatenaccioStore.API.Controllers
{
    [Route("errors/{code}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : BaseController
    {
        public ActionResult Error(int code)
        {
            return new ObjectResult(new ApiResponse(code));
        }
    }
}
