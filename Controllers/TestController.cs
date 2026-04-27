using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAnalisis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("publico")]
        public IActionResult Publico()
        {
            return Ok("Este endpoint es público");
        }

        [Authorize]
        [HttpGet("privado")]
        public IActionResult Privado()
        {
            return Ok("Entraste con token válido 😎");
        }
    }
}