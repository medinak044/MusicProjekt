using MP_API.Data.Models;

namespace MP_API.Data.DTOs;

public class ProjectReqDto
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string OwnerId { get; set; }
}
