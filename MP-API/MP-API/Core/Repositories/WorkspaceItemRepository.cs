using MP_API.Core.Interfaces;
using MP_API.Data;
using MP_API.Data.Models;

namespace MP_API.Core.Repositories;

public class WorkspaceItemRepository: GenericRepository<WorkspaceItem>, IWorkspaceItemRepository
{
    private readonly DataContext _context;

    public WorkspaceItemRepository(DataContext context) : base(context)
    {
        _context = context;
    }
}
