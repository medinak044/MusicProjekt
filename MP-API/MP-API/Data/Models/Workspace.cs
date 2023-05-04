using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MP_API.Data.Models;

public class Workspace
{
    [Key]
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    [ForeignKey("AppUser")]
    public string OwnerId { get; set; }
    [NotMapped] public AppUser? Owner { get; set; } // Determine if this is needed
    [NotMapped] public ICollection<WorkspaceItem>? WorkspaceItems { get; set; }

}
