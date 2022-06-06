using CC.Authority.Implementation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CC.Authority.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly CrisesControlAuthContext _authContext;

        public UsersController(CrisesControlAuthContext authContext)
        {
            _authContext = authContext;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            return Json(_authContext.Users.ToArray());
        }
    }
}