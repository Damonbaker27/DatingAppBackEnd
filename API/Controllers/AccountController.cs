using API.Data;
using API.DTO;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController :BaseApiController
    {       
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
 
        public AccountController( ITokenService tokenService, IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {  
            _tokenService = tokenService;
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }


        [HttpPost("register")] //  POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("Username is taken");
            }

            var user = _mapper.Map<AppUser>(registerDto);
                      
            using var hmac = new HMACSHA512();

            user.UserName = registerDto.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            _userRepository.AddUser(user);

            await _userRepository.SaveAllAsync();

            return new UserDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Token = _tokenService.CreateToken(user),
                Gender = user.Gender
            };

        }
    
        [HttpPost("login")] // POST: api/account/login
        public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
        {

            var user = await _userRepository.GetByNameAsync(loginDto.UserName);

            if (user == null)
            {
                return Unauthorized("Invalid Username");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Incorrect Password");
            }

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                Gender = user.Gender
                 
            };

        }


        [HttpDelete("delete/{username}")]
        public async Task<ActionResult> Delete(string username)
        {
            var user = await _userRepository.GetByNameAsync(username);

            if (user == null)
            {
                return BadRequest("User Does not exist.");
            }

            //delete each photo
            foreach (var photo in user.Photos)
            {
                if(photo.PublicId != null)
                {
                   await _photoService.DeletePhotoAsync(photo.PublicId);
                }           

            }

            _userRepository.DeleteUser(user);

            if (await _userRepository.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("there was an error deleting account");

        }


        private async Task<bool> UserExists(string userName)
        {
            return await _userRepository.UserExist(userName);      
        }


    }
}
