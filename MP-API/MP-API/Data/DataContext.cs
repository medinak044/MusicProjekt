using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MP_API.Data.Models;

namespace MP_API.Data;

public class DataContext: IdentityDbContext<AppUser>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    #region Tables
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<WorkspaceItem> WorkspaceItems { get; set; }
    public DbSet<Project> Projects { get; set; }
    //public DbSet<RefreshToken> RefreshTokens { get; set; }
    #endregion
}
