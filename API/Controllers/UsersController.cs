using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{

    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        //dependancy injection for data context
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            return Ok(await _userRepository.GetMembersAsync());
                           
        }


        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);           
          
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {   
            var user = await _userRepository.GetByNameAsync(User.GetUsername());

            if (user == null) return NotFound();

            //updating user entity properties with new dto ones.
            _mapper.Map(memberUpdateDto, user);

            //return a 204 status code
            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("User updated failed.");    

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>>AddPhoto(IFormFile file)
        {
            //checks if user exists
            var user = await _userRepository.GetByNameAsync(User.GetUsername());
            if (user == null) return NotFound();

            //check if photo upload is successful.
            var result = await _photoService.AddPhotoAsync(file);
            
            if( result.Error!= null ) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            //checks if photo is the first one
            if(user.Photos.Count == 0)
            {
                photo.IsMain= true;
            }
           
            user.Photos.Add(photo);

            //return the mapped Photo DTO
            if (await _userRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUser), 
                    new { username = user.UserName }, _mapper.Map<PhotoDTO>(photo));
            }
            
            
            return BadRequest("Issue while adding photo");


        }



        
    }
}
