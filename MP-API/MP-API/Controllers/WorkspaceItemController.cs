using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.CodeAnalysis;
using MP_API.Data;
using MP_API.Data.DTOs;
using MP_API.Data.Models;
using MP_API.Helpers;

namespace MP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class WorkspaceItemController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkspaceItemController(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet($"{nameof(this.GetAllWorkspaceItems)}")]
        public async Task<ActionResult> GetAllWorkspaceItems()
        {
            var workspaceItems = await _unitOfWork.WorkspaceItems.GetAllAsync();
            return Ok(new ApiResponse()
            {
                DataObject = workspaceItems,
                Success = true,
                Messages = null
            });
        }

        [HttpGet($"{nameof(this.GetWorkspaceItemById)}")]
        public async Task<ActionResult> GetWorkspaceItemById(int workspaceItemId)
        {
            var workspaceItem = await _unitOfWork.WorkspaceItems.GetByIdAsync(workspaceItemId);

            // Check if data was retrieved from DB
            if (workspaceItem == null) 
                return NotFound();
            

            return Ok(new ApiResponse()
            {
                DataObject = workspaceItem,
                Success = true,
                Messages = null
            });
        }

        [HttpGet($"{nameof(this.GetWorkspaceItemsByWorkspaceId)}")]
        public async Task<ActionResult> GetWorkspaceItemsByWorkspaceId(int workspaceId)
        {
            // Check if Workspace exists in DB
            if (await _unitOfWork.Workspaces.GetByIdAsync(workspaceId) == null)
            {
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Messages = new List<string>() { "Workspace does not exist" }
                });
            }

            // Create a collection of all WorkspaceItems associated with the Workspace
            var resultList = new List<WorkspaceItemResDto>();

            // List of participants (AppUsers)
            #region Prepare data for sending user's WorkspaceItems
            var workspaceItems = await _unitOfWork.WorkspaceItems.GetAllAsync();
            foreach (var workspaceItem in workspaceItems)
            {
                // Add all WorkspaceItems that have a matching Workspace id
                if (workspaceItem.WorkspaceId == workspaceId)
                {
                    var resultWorkspaceItem = _mapper.Map<WorkspaceItemResDto>(workspaceItem);
                    // Include Project data
                    resultWorkspaceItem.Project = await _unitOfWork.Projects.GetByIdAsync(resultWorkspaceItem.ProjectId);
                    resultList.Add(resultWorkspaceItem);
                }
            }
            #endregion

            return Ok(new ApiResponse()
            {
                DataObject = resultList,
                Success = true
            });
        }

        [HttpPost($"{nameof(this.CreateWorkspaceItem)}")]
        public async Task<ActionResult> CreateWorkspaceItem(WorkspaceItemReqDto workspaceItemReqDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if Workspace exists in DB
            if (await _unitOfWork.Workspaces.ExistsAsync
                (x => x.Id == workspaceItemReqDto.WorkspaceId) == false)
            {
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Messages = new List<string>() { "Workspace does not exist" }
                });
            }

            //// Check if Project exists in DB
            //if (await _unitOfWork.Projects.ExistsAsync
            //    (x => x.Id == workspaceItemReqDto.ProjectId) == false)
            //{
            //    return BadRequest(new ApiResponse()
            //    {
            //        Success = false,
            //        Messages = new List<string>() { "Project does not exist" }
            //    });
            //}

            // Map DTO to model
            var workspaceItem = _mapper.Map<WorkspaceItem>(workspaceItemReqDto);
            // Set the timestamp
            workspaceItem.DateCreated = DateTime.Now;
            // Set ProjectStatus if it hasn't already been set by client
            if (string.IsNullOrEmpty(workspaceItem.ProjectStatus) == true)
            {
                workspaceItem.ProjectStatus = ProjectStatuses.ProjectStatusEnum.NotStarted.ToString();
            }
            // TODO: Set the PriorityNumber to an integer greater than the current greatest PriorityNumber

            // Save to DB
            await _unitOfWork.WorkspaceItems.AddAsync(workspaceItem);
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
                DataObject = workspaceItem,
                Success = true,
                Messages = new List<string>() { "Creation complete" }
            });
        }

        [HttpPatch($"{nameof(this.UpdateWorkspaceItem)}")]
        public async Task<ActionResult> UpdateWorkspaceItem([FromBody] WorkspaceItemReqDto workspaceItemReqDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if exists in DB
            if (await _unitOfWork.WorkspaceItems.ExistsAsync(x => x.Id == workspaceItemReqDto.Id) == false)
            {
                return NotFound(new ApiResponse()
                {
                    DataObject = null,
                    Success = false,
                    Messages = new List<string>() { "WorkspaceItem not found" }
                });
            }

            // Check if Workspace exists
            if (await _unitOfWork.Workspaces.ExistsAsync(x => x.Id == workspaceItemReqDto.WorkspaceId) == false)
            {
                return NotFound(new ApiResponse()
                {
                    DataObject = null,
                    Success = false,
                    Messages = new List<string>() { "Workspace not found" }
                });
            }

            // Check if Project exists
            if (await _unitOfWork.Projects.ExistsAsync(x => x.Id == workspaceItemReqDto.ProjectId) == false)
            {
                return NotFound(new ApiResponse()
                {
                    DataObject = null,
                    Success = false,
                    Messages = new List<string>() { "Project not found" }
                });
            }

            // Map DTO
            var workspaceItem = _mapper.Map<WorkspaceItem>(workspaceItemReqDto);
            // Include the id
            workspaceItem.Id = workspaceItemReqDto.Id;
            // TODO: Check if the ProjectStatus string value matches one of the terms from the DB
            if (string.IsNullOrEmpty(workspaceItemReqDto.ProjectStatus) == true)
            {
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Messages = new List<string>() { "ProjectStatus does not have a value" }
                });
            }
            workspaceItem.ProjectStatus = workspaceItemReqDto.ProjectStatus;

            // Save to DB
            await _unitOfWork.WorkspaceItems.UpdateAsync(workspaceItem);
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
                DataObject = workspaceItem, // Updated data
                Success = true,
                Messages = new List<string>() { "Update complete" }
            });
        }

        [HttpDelete($"{nameof(this.DeleteWorkspaceItem)}")]
        public async Task<ActionResult> DeleteWorkspaceItem(int workspaceItemId)
        {
            // Track the entity to be deleted
            var workspaceItem = await _unitOfWork.WorkspaceItems.GetByIdAsync(workspaceItemId);

            if (workspaceItem == null)
                return NotFound();

            // Delete from DB
            await _unitOfWork.WorkspaceItems.DeleteAsync(workspaceItem);
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
                DataObject = workspaceItem, // Sending previous data allows client to undo changes
                Success = true,
                Messages = new List<string>() { "Succesfully deleted" }
            });
        }
    }
}
