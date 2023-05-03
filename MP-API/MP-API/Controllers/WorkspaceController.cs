using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkspaceController(
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet($"{nameof(this.GetAllWorkspaces)}")]
        public async Task<ActionResult> GetAllWorkspaces()
        {
            var result = await _unitOfWork.Workspaces.GetAllAsync();
            return Ok(new ApiResponse()
            {
                DataObject = result,
                Success = true,
                Messages = null
            });
        }

        [HttpGet($"{nameof(this.GetWorkspaceByUserId)}")]
        public async Task<ActionResult> GetWorkspaceByUserId(int userId)
        {
            return Ok();
        }

        [HttpGet($"{nameof(this.GetUserWorkspaces)}")]
        public async Task<ActionResult> GetUserWorkspaces(int userId)
        {
            
            return Ok();
        }

        [HttpPost($"{nameof(this.CreateWorkspace)}")]
        public async Task<ActionResult> CreateWorkspace(WorkspaceReqDto workspaceReqDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map DTO
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
        public async Task<ActionResult> UpdateWorkspace([FromRoute] int workspaceId, [FromBody] WorkspaceReqDto workspaceReqDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if exists in DB
            if (await _unitOfWork.Workspaces.ExistsAsync(x => x.Id == workspaceId) == false)
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
            workspace.Id = workspaceId;

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
                DataObject = workspace,
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

            return Ok(workspace);
        }
    }
}
