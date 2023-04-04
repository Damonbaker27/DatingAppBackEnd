using System;
using FakeItEasy;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Interface;
using API.Services;
using AutoMapper;
using API.DTO;
using API.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using API.Entities;

namespace DatingApp.Tests.UsersController
{
    public class UsersControllerTests
    {

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersControllerTests()
        {
            _userRepository = A.Fake<IUserRepository>();
            _mapper = A.Fake<IMapper>();
            _photoService = A.Fake<IPhotoService>();
        }

        [Fact]
        public void UsersController_GetUsers_ReturnOk()
        {
            //Arrange
            var members = A.Fake<ICollection<MemberDTO>>();
            var memberList = A.Fake<IEnumerable<MemberDTO>>();

            A.CallTo(() => _mapper.Map<IEnumerable<MemberDTO>>(members)).Returns(memberList);
            var controller = new API.Controllers.UsersController(_userRepository, _mapper, _photoService);

            //Act
            //var result = controller.GetUsers();


            //Assert
            //result.Should().NotBeNull();         
        }


        [Fact]
        public void UserController_GetUser_ReturnActionResult()
        {
            //Arrange
            var member = A.Fake<AppUser>();
            var memberCreate = A.Fake<MemberDTO>();

      

            var controller = new API.Controllers.UsersController(_userRepository, _mapper, _photoService);

            //Act 
            var result = controller.GetUser(member.UserName);


            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(MemberDTO));

        }






    }
}
