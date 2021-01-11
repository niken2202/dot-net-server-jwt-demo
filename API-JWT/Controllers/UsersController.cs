using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_JWT.Entities;
using API_JWT.Models;
using API_JWT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_JWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost("login")]
        public ActionResult<AuthenticateResponse> Post(AuthenticateRequest request)
        {
            try
            {
                var aures = _userService.Authenticate(request);
                return aures;
            } catch(Exception x)
            {
                return Ok(x.Message);
            }
             
        }

        // GET api/values/5
        [HttpPost("token")]
        public string GetToken(AuthenticateRequest request)
        {
            var token = string.Empty;
            var user = _userService.GetAll()
                                    .FirstOrDefault(x => x.Username == request.Username &&
                                        x.Password == request.Password
                                    );
            if (user != null)
            {
                token = _userService.generateToken(user);
            }
            return token;
        }

        [HttpPost("renew-token")]
        public ActionResult<AuthenticateResponse> RenewToken(RefreshTokenRequest request)
        {
            try
            {
                var newToken = _userService.RenewToken(request.Token, request.RefreshToken);
                return newToken;

            }
            catch(Exception e)
            {
                return Ok(e.Message);
            };
           
        }

        [Authorize]
        [HttpGet("getall")]
        public ActionResult<User> GetAll()
        {
            return Ok(_userService.GetAll());
        }

    }
}
