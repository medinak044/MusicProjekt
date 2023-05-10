using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MP_API.Data;
using MP_API.Data.DTOs;
using MP_API.Data.Models;

namespace MP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class WorkspaceController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkspaceController(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet($"{nameof(this.GetAllWorkspaces)}")]
        public async Task<ActionResult> GetAllWorkspaces()
        {
            var workspaces = await _unitOfWork.Workspaces.GetAllAsync();
            return Ok(new ApiResponse()
            {
                DataObject = workspaces,
                Success = true,
                Messages = null
            });
        }

        [HttpGet($"{nameof(this.GetWorkspaceById)}")]
        public async Task<ActionResult> GetWorkspaceById(int workspaceId)
        {
            var workspace = await _unitOfWork.Workspaces.GetByIdAsync(workspaceId);
            return Ok(new ApiResponse()
            {
                DataObject = workspace,
                Success = true,
                Messages = null
            });
        }

        [HttpGet($"{nameof(this.GetUserWorkspaces)}")]
        public async Task<ActionResult> GetUserWorkspaces(string userId)
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

            var resultList = new List<WorkspaceResDto>();

            #region Prepare data for sending user's Workspaces
            var workspaces = await _unitOfWork.Workspaces.GetAllAsync();
            foreach (var workspace in workspaces)
            {
                // Add all Workspaces that have a matching owner id
                if (workspace.OwnerId == userId)
                {
                    var resultWorkspace = _mapper.Map<WorkspaceResDto>(workspace);
                    // Include collection object data based on workspace id
                    resultWorkspace.WorkspaceItems = _unitOfWork.WorkspaceItems.GetSome(wI => wI.WorkspaceId == resultWorkspace.Id).ToList();
                    // Include owner object data based on owner id
                    resultWorkspace.Owner = _mapper.Map<AppUserDto>(await _userManager.FindByIdAsync(resultWorkspace.OwnerId));
                    resultList.Add(resultWorkspace);
                }
            }
            #endregion

            return Ok(new ApiResponse()
            {
                DataObject = resultList,
                Success = true
            });
        }

        [HttpPost($"{nameof(this.CreateWorkspace)}")]
        public async Task<ActionResult> CreateWorkspace(WorkspaceReqDto workspaceReqDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if user exists in DB
            if (await _userManager.FindByIdAsync(workspaceReqDto.OwnerId) == null)
            {
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Messages = new List<string>() { "User does not exist" }
                });
            }

            // Map DTO to model
            var workspace = _mapper.Map<Workspace>(workspaceReqDto);
            

            // Save to DB
            await _unitOfWork.Workspaces.AddAsync(workspace);
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
                DataObject = workspace,
                Success = true,
                Messages = new List<string>() { "Creation complete" }
            });
        }

        [HttpPatch($"{nameof(this.UpdateWorkspace)}")]
        public async Task<ActionResult> UpdateWorkspace([FromBody] WorkspaceReqDto workspaceReqDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if exists in DB
            if (await _unitOfWork.Workspaces.ExistsAsync(x => x.Id == workspaceReqDto.Id) == false)
            {
                return NotFound(new ApiResponse()
                {
                    DataObject = null,
                    Success = false,
                    Messages = new List<string>() { "Not found" }
                });
            }

            // Map DTO
            var workspace = _mapper.Map<Workspace>(workspaceReqDto);
            // Include the id
            workspace.Id = workspaceReqDto.Id;

            // Save to DB
            await _unitOfWork.Workspaces.UpdateAsync(workspace);
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
                DataObject = workspace, // Updated data
                Success = true,
                Messages = new List<string>() { "Update complete" }
            });
        }

        [HttpDelete($"{nameof(this.DeleteWorkspace)}")]
        public async Task<ActionResult> DeleteWorkspace(int workspaceId)
        {
            // Track the entity to be deleted
            var workspace = await _unitOfWork.Workspaces.GetByIdAsync(workspaceId);

            if (workspace == null)
                return NotFound();

            // Delete from DB
            await _unitOfWork.Workspaces.DeleteAsync(workspace);
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
                DataObject = workspace, // Sending previous data allows client to undo changes
                Success = true,
                Messages = new List<string>() { "Succesfully deleted" }
            });
        }
    }
}
