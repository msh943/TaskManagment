using TaskManagment.Domain.Enums;

namespace TaskManagment.Service.Dtos
{
    public record TaskCreateDto(string Title, string? Description, string AssignedUserId, TaskStatuses Status);
    public record TaskUpdateDto(string? Title, string? Description, string? AssignedUserId, TaskStatuses? Status);
    public record TaskStatusUpdateDto(TaskStatuses Status);
    public record TaskDto(int Id, string Title, string? Description, TaskStatuses Status, string AssignedUserId, string? AssignedUserEmail,  string? AssignedUserFullName);
}
