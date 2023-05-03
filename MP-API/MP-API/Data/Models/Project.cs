using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MP_API.Data.Models;

public class Project
{
    [Key]
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [ForeignKey("AppUser")]
    public int OwnerId { get; set; }
    [NotMapped]
    public AppUser? Owner { get; set; }

}
