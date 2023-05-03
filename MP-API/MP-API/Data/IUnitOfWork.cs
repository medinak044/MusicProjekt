using MP_API.Core.Interfaces;

namespace MP_API.Data;

public interface IUnitOfWork: IDisposable
{
    Task<bool> SaveAsync();
    //IAppUserRepository AppUsers { get; }
    IWorkspaceRepository Workspaces { get; }
    IWorkspaceItemRepository WorkspaceItems { get; }
    IProjectRepository Projects { get; }


}
