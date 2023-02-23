using API.Data;
using API.DTO;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;

        //dependancy injection for data context
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            return Ok(await _userRepository.GetUsersAsync());
           
        }


        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            return Ok(await _userRepository.GetByNameAsync(username));
          
        }


        
    }
}
