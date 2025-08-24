using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagment.Api.Controllers;
using TaskManagment.Domain.Entities;
using TaskManagment.Domain.Enums;
using TaskManagment.Infrastructure.Repositories;
using TaskManagment.Infrastructure.UnitOfWork;
using TaskManagment.Service;
using TaskManagment.Service.Dtos;
using TaskManagment.Service.IServices;
using TaskManagment.Service.Services;

namespace TaskManager.Tests
{
    public class TasksControllerTests
    {
        private readonly IMapper _mapper;

        public TasksControllerTests() => _mapper = MappingFixture.CreateMapper(typeof(MappingProfile));
        [Fact]
        public async Task GetMyTasksAsync_ReturnsOnlyAssignedTasks_ForCurrentUser()
        {
            // arrange
            var userId = "u-1";
            var principal = FakePrincipal.User(userId);

            var tasks = new List<TaskItems>
            {
                new TaskItems { Id = 1, Title = "A", Status = TaskStatuses.New,        AssignedUserId = userId },
                new TaskItems { Id = 2, Title = "B", Status = TaskStatuses.InProgress, AssignedUserId = userId },
                new TaskItems { Id = 3, Title = "C", Status = TaskStatuses.Done,       AssignedUserId = "u-2"  },
            };

            var repo = new Mock<ITaskRepository>();
            repo.Setup(r => r.GetByAssignedUserAsync(userId))
                .ReturnsAsync(tasks.Where(t => t.AssignedUserId == userId).ToList());

            var uow = new Mock<IUnitOfWork>();
            uow.SetupGet(u => u.Tasks).Returns(repo.Object);

            var sut = new TaskService(uow.Object, _mapper);

            // act
            var result = (await sut.GetMyTasksAsync(principal)).ToList();

            // assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(t => t.AssignedUserId == userId); // <-- key check
            result.Select(t => t.Id).Should().BeEquivalentTo(new[] { 1, 2 }, o => o.WithoutStrictOrdering());
        }
    }
}
