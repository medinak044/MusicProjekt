﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.CodeAnalysis;
using MP_API.Data;
using MP_API.Data.DTOs;
using MP_API.Data.Models;

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
            return Ok(new ApiResponse()
            {
                DataObject = workspaceItem,
                Success = true,
                Messages = null
            });
        }

        [HttpGet($"{nameof(this.GetWorkspaceItems)}")]
        public async Task<ActionResult> GetWorkspaceItems(int workspaceId)
        {
            // Check if Workspace exists in DB
            if (await _unitOfWork.WorkspaceItems.GetByIdAsync(workspaceId) == null)
            {
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Messages = new List<string>() { "User does not exist" }
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
                    // Include collection object data based on workspaceItem id
                    //resultWorkspaceItem.Participants = _mapper.Map<AppUserDto>(await _userManager.FindByIdAsync(resultWorkspaceItem.OwnerId)); /*_unitOfWork.WorkspaceItems.GetSome(wI => wI.WorkspaceItemId == resultWorkspaceItem.Id).ToList();*/
                    // Include owner object data based on owner id
                    //resultWorkspaceItem.Owner = _mapper.Map<AppUserDto>(await _userManager.FindByIdAsync(resultWorkspaceItem.OwnerId));
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

            // Check if workspace exists in DB
            if (await _userManager.FindByIdAsync(workspaceItemReqDto.OwnerId.ToString()) == null)
            {
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Messages = new List<string>() { "User does not exist" }
                });
            }

            // Map DTO to model
            var workspaceItem = _mapper.Map<WorkspaceItem>(workspaceItemReqDto);
            // Set the timestamp
            workspaceItem.DateCreated = DateTime.Now;

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
        public async Task<ActionResult> UpdateWorkspaceItem(int workspaceItemId, [FromBody] WorkspaceItemReqDto workspaceItemReqDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if exists in DB
            if (await _unitOfWork.WorkspaceItems.ExistsAsync(x => x.Id == workspaceItemId) == false)
            {
                return NotFound(new ApiResponse()
                {
                    DataObject = null,
                    Success = false,
                    Messages = new List<string>() { "Not found" }
                });
            }

            // Map DTO
            var workspaceItem = _mapper.Map<WorkspaceItem>(workspaceItemReqDto);
            // Include the id
            workspaceItem.Id = workspaceItemId;

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