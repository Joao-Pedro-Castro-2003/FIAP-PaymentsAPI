using Microsoft.AspNetCore.Mvc;
namespace PaymentsAPI.Controllers;

[ApiController,Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new {service = "PaymentsAPI", status = "healthy"});
}
