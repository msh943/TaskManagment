using System.ComponentModel.DataAnnotations;
using TaskManagment.Domain.Enums;
using TaskManagment.Domain.Identity;

namespace TaskManagment.Domain.Entities
{
    public class TaskItems : BaseEntity
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatuses Status { get; set; } = TaskStatuses.New;
        [Required]
        public string AssignedUserId { get; set; } = string.Empty;
        public AppUser? AssignedUser { get; set; }
    }
}
