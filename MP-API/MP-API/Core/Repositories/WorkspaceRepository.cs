using MP_API.Core.Interfaces;
using MP_API.Data;
using MP_API.Data.Models;

namespace MP_API.Core.Repositories;

public class WorkspaceRepository: GenericRepository<Workspace>, IWorkspaceRepository
{
    private readonly DataContext _context;

    public WorkspaceRepository(DataContext context) : base(context)
    {
        _context = context;
    }
}
