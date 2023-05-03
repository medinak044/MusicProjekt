using MP_API.Core.Interfaces;
using MP_API.Data;
using MP_API.Data.Models;

namespace MP_API.Core.Repositories;

public class ProjectRepository: GenericRepository<Project>, IProjectRepository
{
    private readonly DataContext _context;

    public ProjectRepository(DataContext context) : base(context)
    {
        _context = context;
    }
}
