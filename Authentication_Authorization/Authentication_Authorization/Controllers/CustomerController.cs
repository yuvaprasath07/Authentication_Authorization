using Authentication_Authorization.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Authentication_Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetPathName")]
        public IActionResult GetData()
        {
            var data = _context.pathDetails.ToList();
            return Ok(data);
        }
    }
}
