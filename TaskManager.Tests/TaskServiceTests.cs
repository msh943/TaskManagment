using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManagment.Domain.Entities;
using TaskManagment.Domain.Enums;
using TaskManagment.Infrastructure.Repositories;
using TaskManagment.Infrastructure.UnitOfWork;
using TaskManagment.Service;
using TaskManagment.Service.Services;

namespace TaskManager.Tests
{
    public class TaskServiceTests
    {
        private readonly IMapper _mapper;

        public TaskServiceTests() => _mapper = MappingFixture.CreateMapper(typeof(MappingProfile));

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

            // ✅ key check: every returned task belongs to the current user (string id)
            result.Should().OnlyContain(t => t.AssignedUserId == userId);

            // ✅ check the numeric Task IDs (not user ids)
            result.Select(t => t.Id)   // or .id if your DTO uses lowercase
                  .Should().BeEquivalentTo(new[] { 1, 2 }, o => o.WithoutStrictOrdering());
        }

    }
}
