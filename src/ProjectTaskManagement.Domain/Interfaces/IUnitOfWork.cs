using ProjectTaskManagement.Domain.Entities;

namespace ProjectTaskManagement.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Project> Projects { get; }
    IRepository<TaskItem> Tasks { get; }
    IRepository<ApplicationUser> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
