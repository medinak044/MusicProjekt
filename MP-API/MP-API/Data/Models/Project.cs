using MP_API.Data.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MP_API.Data.Models;

public class Project
{
    [Key]
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    [ForeignKey("AppUser")]
    public string OwnerId { get; set; }
    [NotMapped] public AppUserDto? Owner { get; set; }

}
