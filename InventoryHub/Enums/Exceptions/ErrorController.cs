namespace InventoryHub.Exceptions
{
    using InventoryHub.Responses;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error")]
    public class ErrorController : ControllerBase
    {
        public IActionResult HandleError()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var message = exception?.Error.Message ?? "Unexpected error";

            var response = ResponseFactory.Fail<object>(message);
            return StatusCode(500, response);
        }
    }
}
