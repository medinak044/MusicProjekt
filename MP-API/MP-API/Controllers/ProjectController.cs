using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MP_API.Data.Models;
using MP_API.Data;
using MP_API.Data.DTOs;

namespace MP_API.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("fixed")]
public class ProjectController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectController(
        UserManager<AppUser> userManager,
        IUnitOfWork unitOfWork,
        IMapper mapper
        )
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet($"{nameof(this.GetAllProjects)}")]
    public async Task<ActionResult> GetAllProjects()
    {
        var projects = await _unitOfWork.Projects.GetAllAsync();
        return Ok(new ApiResponse()
        {
            DataObject = projects,
            Success = true,
            Messages = null
        });
    }

    [HttpGet($"{nameof(this.GetProjectById)}")]
    public async Task<ActionResult> GetProjectById(int projectId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId);

        // Check if data was retrieved from DB
        if (project == null)
            return NotFound();

        return Ok(new ApiResponse()
        {
            DataObject = project,
            Success = true,
            Messages = null
        });
    }

    [HttpGet($"{nameof(this.GetUserProjects)}")]
    public async Task<ActionResult> GetUserProjects(string userId)
    {
        // Check if user exists in DB
        if (await _userManager.FindByIdAsync(userId) == null)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "User does not exist" }
            });
        }

        var resultList = new List<ProjectResDto>();

        #region Prepare data for sending user's Projects
        var projects = await _unitOfWork.Projects.GetAllAsync();
        foreach (var project in projects)
        {
            // Add all Projects that have a matching owner id
            if (project.OwnerId == userId)
            {
                var resultProject = _mapper.Map<ProjectResDto>(project);
                // Include owner object data based on owner id
                resultProject.Owner = _mapper.Map<AppUserDto>(await _userManager.FindByIdAsync(resultProject.OwnerId));
                resultList.Add(resultProject);
            }
        }
        #endregion

        return Ok(new ApiResponse()
        {
            DataObject = resultList,
            Success = true
        });
    }

    [HttpPost($"{nameof(this.CreateProject)}")]
    public async Task<ActionResult> CreateProject(ProjectReqDto projectReqDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if user exists in DB
        if (await _userManager.FindByIdAsync(projectReqDto.OwnerId) == null)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "User does not exist" }
            });
        }

        // Map DTO to model
        var project = _mapper.Map<Project>(projectReqDto);
        // Set the timestamp
        project.DateCreated = DateTime.Now;

        // Save to DB
        await _unitOfWork.Projects.AddAsync(project);
        if (await _unitOfWork.SaveAsync() == false)
        {
            return BadRequest(new ApiResponse()
            {
                DataObject = null,
                Success = false,
                Messages = new List<string>() { "Something went wrong while saving" }
            });
        }

        return Ok(new ApiResponse()
        {
            DataObject = project,
            Success = true,
            Messages = new List<string>() { "Creation complete" }
        });
    }

    [HttpPatch($"{nameof(this.UpdateProject)}")]
    public async Task<ActionResult> UpdateProject([FromBody] ProjectReqDto projectReqDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if exists in DB
        if (await _unitOfWork.Projects.ExistsAsync(x => x.Id == projectReqDto.Id) == false)
        {
            return NotFound(new ApiResponse()
            {
                DataObject = null,
                Success = false,
                Messages = new List<string>() { "Not found" }
            });
        }

        // Map DTO
        var project = _mapper.Map<Project>(projectReqDto);
        // Include the id
        project.Id = projectReqDto.Id;

        // Save to DB
        await _unitOfWork.Projects.UpdateAsync(project);
        if (await _unitOfWork.SaveAsync() == false)
        {
            return BadRequest(new ApiResponse()
            {
                DataObject = null,
                Success = false,
                Messages = new List<string>() { "Something went wrong while saving" }
            });
        }

        return Ok(new ApiResponse()
        {
            DataObject = project, // Updated data
            Success = true,
            Messages = new List<string>() { "Update complete" }
        });
    }

    [HttpDelete($"{nameof(this.DeleteProject)}")]
    public async Task<ActionResult> DeleteProject(int projectId)
    {
        // Track the entity to be deleted
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId);

        if (project == null)
            return NotFound();

        // Delete from DB
        await _unitOfWork.Projects.DeleteAsync(project);
        if (await _unitOfWork.SaveAsync() == false)
        {
            return BadRequest(new ApiResponse()
            {
                DataObject = null,
                Success = false,
                Messages = new List<string>() { "Something went wrong while saving" }
            });
        }

        return Ok(new ApiResponse()
        {
            DataObject = project, // Sending previous data allows client to undo changes
            Success = true,
            Messages = new List<string>() { "Succesfully deleted" }
        });
    }
}
