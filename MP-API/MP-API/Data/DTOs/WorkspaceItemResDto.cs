using MP_API.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MP_API.Data.DTOs;

public class WorkspaceItemResDto
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public int PriorityNumber { get; set; }
    public string ProjectStatus { get; set; } // Set by enum
    public int WorkspaceId { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    public ICollection<AppUserDto>? Participants { get; set; } // Set owner as Participant by default
}
