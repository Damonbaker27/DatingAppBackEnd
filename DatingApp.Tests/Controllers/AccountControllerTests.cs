using API;
using API.Controllers;
using API.Data;
using API.DTO;
using API.Entities;
using API.Helper;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace DatingApp.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<DataContext> _context;
        private readonly Mock<ITokenService> _tokenService;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly IMapper _mapper;
        private readonly Mock<IPhotoService> _photoService;

        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _context = new Mock<DataContext>();
            _tokenService = new Mock<ITokenService>();
            _userRepository = new Mock<IUserRepository>();
            _photoService = new Mock<IPhotoService>();

            var myProfile = new AutoMapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);



            _controller = new AccountController(_tokenService.Object, 
                _userRepository.Object, mapper, _photoService.Object);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsUserDto()
        {
            // Arrange

            var registerDto = new RegisterDto
            {
                UserName = "testuser",
                Password = "testpassword",
                KnownAs = "Test User"
            };

            var user = new AppUser
            {
                UserName = registerDto.UserName,
                KnownAs = registerDto.KnownAs
            };


            var userDto = new UserDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Token = "testtoken"
            };
            
            
            



            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<ActionResult<UserDto>>(result);
            var dto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userDto.UserName, dto.UserName);
            Assert.Equal(userDto.KnownAs, dto.KnownAs);
            Assert.Equal(userDto.Token, dto.Token);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsUserDto()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "testuser",
                Password = "testpassword"
            };
            var user = new AppUser
            {
                UserName = loginDto.UserName,
                PasswordHash = new HMACSHA512().ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)),
                PasswordSalt = new HMACSHA512().Key
            };
            var userDto = new UserDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Token = "testtoken",
                PhotoUrl = "testurl"
            };
            _userRepository.Setup(u => u.GetByNameAsync(loginDto.UserName)).ReturnsAsync(user);
            _tokenService.Setup(t => t.CreateToken(user)).Returns(userDto.Token);
            user.Photos = new List<Photo> { new Photo { Url = "testurl", IsMain = true } };

            // Act
            var result = await _controller.login(loginDto);

            // Assert
            var okResult = Assert.IsType<ActionResult<UserDto>>(result);
            var dto = Assert.IsType<UserDto>(okResult.Value);
            
        }
    }
}
