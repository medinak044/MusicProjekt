using MP_API.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MP_API.Data.DTOs;

public class WorkspaceResDto
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string OwnerId { get; set; }
    public AppUserDto? Owner { get; set; } // Using DTO to not send sensitive user information to client
    public ICollection<WorkspaceItem>? WorkspaceItems { get; set; }
}
