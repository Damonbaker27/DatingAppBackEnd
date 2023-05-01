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
using API.Helper;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DatingApp.Tests.UsersController
{
    public class UsersControllerTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private Mock<IPhotoService> _photoServiceMock;
        private API.Controllers.UsersController _controller;

        public UsersControllerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _photoServiceMock = new Mock<IPhotoService>();

            _controller = new API.Controllers.UsersController(_userRepositoryMock.Object, _mapperMock.Object, _photoServiceMock.Object);

            // Set the current user for the test
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, "testuser")
                }, "TestAuthentication"))
                }
            };
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResult_WhenUsersExist()
        {
            // Arrange
            var userParams = new UserParams { Gender = "female" };

            var currentUser = new AppUser { UserName = "testuser", Gender = "male" };

            _userRepositoryMock.Setup(repo => repo.GetByNameAsync("testuser"))
                .ReturnsAsync(currentUser);

            var users = new PagedList<MemberDTO>(new List<MemberDTO>(), 1, 10, 2);
            _userRepositoryMock.Setup(repo => repo.GetMembersAsync(userParams))
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetUsers(userParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagedList = Assert.IsType<PagedList<MemberDTO>>(okResult.Value);
            Assert.Equal(users, pagedList);
        }

        [Fact]
        public async Task GetUsers_AddsPaginationHeader_WhenUsersExist()
        {
            // Arrange
            var userParams = new UserParams { Gender = "female" };
            var currentUser = new AppUser { UserName = "testuser", Gender = "male" };
            _userRepositoryMock.Setup(repo => repo.GetByNameAsync("testuser"))
                .ReturnsAsync(currentUser);
            var users = new PagedList<MemberDTO>(new List<MemberDTO>(), 1, 10, 2);
            _userRepositoryMock.Setup(repo => repo.GetMembersAsync(userParams))
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetUsers(userParams);

            // Assert
            _controller.HttpContext.Response.Headers.TryGetValue("Pagination", out var paginationHeaderValues);
            Assert.NotNull(paginationHeaderValues);
            Assert.Single(paginationHeaderValues);
            var paginationHeader = JsonConvert.DeserializeObject<PaginationHeader>(paginationHeaderValues.First());
            Assert.Equal(users.CurrentPage, paginationHeader.CurrentPage);
            Assert.Equal(users.PageSize, paginationHeader.ItemsPerPage);
            Assert.Equal(users.TotalCount, paginationHeader.TotalItems);
            Assert.Equal(users.TotalPages, paginationHeader.TotalPages);
        }




    }
}
