using System.ComponentModel.DataAnnotations.Schema;

namespace MP_API.Data.DTOs;

public class WorkspaceItemReqDto
{
    public DateTime DateCreated { get; set; }
    public int PriorityNumber { get; set; }
    public string? ProjectStatus { get; set; }
    public int WorkspaceId { get; set; }
    public int ProjectId { get; set; }
}
