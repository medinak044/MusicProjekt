﻿namespace MP_API.Data.DTOs;

public class WorkspaceReqDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string OwnerId { get; set; }
}
