using AuthenticationAPI.Application.DTOs;
using AuthenticationAPI.Application.Interfaces;
using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUserRepository userRepository) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register(AppUserDTO appUserDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await userRepository.Register(appUserDTO);
            return result.Flag ? Ok(result) : BadRequest(Request);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await userRepository.Login(loginDTO);
            return result.Flag ? Ok(result) : BadRequest(Request);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetUserDTO>> GetUser(int id)
        {
            if (id <= 0) return BadRequest("Invalid user Id");
            var user = await userRepository.GetUser(id);
            return user == null ? BadRequest(Request) : Ok(user);
        }
    }
}
