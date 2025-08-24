using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagment.Domain.Entities;
using TaskManagment.Domain.Identity;
using TaskManagment.Service.Dtos;

namespace TaskManagment.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaskCreateDto, TaskItems>();


            CreateMap<TaskUpdateDto, TaskItems>()
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<TaskStatusUpdateDto, TaskItems>()
                    .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
                    .ForMember(d => d.Title, opt => opt.Ignore())
                    .ForMember(d => d.Description, opt => opt.Ignore())
                    .ForMember(d => d.AssignedUserId, opt => opt.Ignore());

            CreateMap<TaskItems, TaskDto>()
                .ForMember(d => d.AssignedUserEmail,
                    opt => opt.MapFrom(src => src.AssignedUser != null ? src.AssignedUser.Email : null))
                .ForMember(d => d.AssignedUserFullName,
                    opt => opt.MapFrom(s => s.AssignedUser != null ? s.AssignedUser.FullName : null)); 
 

            CreateMap<CreateUserDto, AppUser>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.PasswordHash, opt => opt.Ignore());


            CreateMap<AppUser, UserDto>()
                .ForMember(d => d.Role, opt => opt.Ignore());
        }
    }
}
