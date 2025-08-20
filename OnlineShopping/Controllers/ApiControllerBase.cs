using Microsoft.AspNetCore.Mvc;

namespace OnlineShopping.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
}