
namespace CatenaccioStore.API.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }


        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad request occured",
                401 => "UnAuthorized, you are not authorized!",
                404 => "Not Found, Resource was not found",
                500 => "Internal Server Error",
                _ => "Unexcpexted Error"
            };
        }
    }
}
