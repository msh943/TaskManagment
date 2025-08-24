using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagment.Api.Controllers;
using TaskManagment.Service.Dtos;
using TaskManagment.Service.IServices;

namespace TaskManager.Tests
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOk_WithUsers()
        {

            var svc = new Mock<IUserService>();
            var users = new List<UserDto>
        {
            new UserDto { Id = "1", Email = "admin@demo.com", FullName = "Admin Demo", Role = "Admin" },
            new UserDto { Id = "2", Email = "user@demo.com",  FullName = "User Demo",  Role = "User" }
        };
            svc.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

            var controller = new UsersController(svc.Object);


            var action = await controller.GetAll();


            action.Result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)action.Result!;
            var payload = ok.Value as IEnumerable<UserDto>;
            payload.Should().NotBeNull();
            payload!.Should().HaveCount(2);
        }
    }
}
