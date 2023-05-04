using AutoMapper;
using Microsoft.Extensions.Logging;
using MP_API.Data.DTOs;
using MP_API.Data.Models;

namespace MP_API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, AppUserDto>(); // Data from api to client
        CreateMap<AppUserDto, AppUser>(); // When updating existing user info from a dto

        CreateMap<AppUserRegistrationDto, AppUser>(); // When registering new user
        CreateMap<AppUser, AppUserLoggedInDto>(); // When logging user in, gives user info

        CreateMap<WorkspaceReqDto, Workspace>(); // When user creates a Workspace
        CreateMap<Workspace, WorkspaceResDto>(); // When sending Workspaces to client


        //CreateMap<AuthResult, AppUserLoggedInDto>(); // When logging user in, gives token for the client

        //CreateMap<AppUser, AppUserAdminDto>(); // When user with "Admin" role gets all users (with more information)
        //CreateMap<AppUser, UserConnectionResponseDto>(); // Basically AppUserAdminDto + userConnectionId

        //CreateMap<EventRequestDto, Event>(); // When user creates an Event
        //CreateMap<Event, EventRequestDto>(); // When user creates an Event
        //CreateMap<Event, EventResponseDto>();
        //CreateMap<EventRequestDto, EventResponseDto>();


        //CreateMap<AttendeeRequestDto, Attendee>(); // When updating an Attendee
    }
}
