using MP_API.Core.Interfaces;
using MP_API.Core.Repositories;

namespace MP_API.Data;

public class UnitOfWork: IUnitOfWork, IDisposable
{
    private readonly DataContext _context;

    public UnitOfWork(DataContext context)
    {
        _context = context;
    }

    public IWorkspaceRepository Workspaces => new WorkspaceRepository(_context);
    public IWorkspaceItemRepository WorkspaceItems => new WorkspaceItemRepository(_context);
    public IProjectRepository Projects => new ProjectRepository(_context);

    public async Task<bool> SaveAsync()
    {
        int saved = await _context.SaveChangesAsync(); // Returns an integer
        return saved > 0 ? true : false;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
