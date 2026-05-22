using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Interfaces;

namespace ProjectTaskManagement.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IRepository<Project>? _projects;
    private IRepository<TaskItem>? _tasks;
    private IRepository<ApplicationUser>? _users;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Project> Projects =>
        _projects ??= new Repository<Project>(_context);

    public IRepository<TaskItem> Tasks =>
        _tasks ??= new Repository<TaskItem>(_context);

    public IRepository<ApplicationUser> Users =>
        _users ??= new Repository<ApplicationUser>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
