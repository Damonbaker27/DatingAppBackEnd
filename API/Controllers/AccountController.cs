using API.Data;
using API.DTO;
using API.Entities;
using API.Interface;
using AutoMapper;
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
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        //Inject datacontext class into controller 
        public AccountController(DataContext context, ITokenService tokenService, IUserRepository userRepository, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _mapper = mapper;
        }


        [HttpPost("register")] //  POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);
                      
            using var hmac = new HMACSHA512();

            user.UserName = registerDto.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return new UserDto 
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Token = _tokenService.CreateToken(user)
            };

        }

        [HttpPost("login")] // POST: api/account/login
        public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
        {

            var user = await _userRepository.GetByNameAsync(loginDto.UserName);

            if (user == null) return Unauthorized("Invalid Username");

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
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };

        }


        private async Task<bool> UserExists(string userName)
        {
            return await _userRepository.UserExist(userName);      
        }


    }
}
