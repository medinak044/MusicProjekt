﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MP_API.Data.Models;

public class WorkspaceItem
{
    [Key]
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public int PriorityNumber { get; set; }
    public string ProjectStatus { get; set; }
    [ForeignKey("Workspace")]
    public int WorkspaceId { get; set; }
    [ForeignKey("Project")]
    public int ProjectId { get; set; }
    [NotMapped] public Project? Project { get; set; }
    [ForeignKey("AppUser")]
    public int OwnerId { get; set; }
    [NotMapped] public AppUser? Owner { get; set; }
    [NotMapped] public ICollection<AppUser>? Participants { get; set; } // Set owner as Participant by default
    //public ICollection<Comment>? Comments { get; set; } // List of comments by participating users regarding the project
}