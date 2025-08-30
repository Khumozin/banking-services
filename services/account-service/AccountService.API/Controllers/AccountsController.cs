using Microsoft.AspNetCore.Mvc;

namespace AccountService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : Controller
{
    [HttpGet("health")]
    public ActionResult HealthCheck()
    {
        return Ok(new { status = "OK", service = "accounts-service", timestamp = DateTime.UtcNow });
    }
}