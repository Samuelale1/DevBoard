// IReadOnlyRepository.cs
namespace DevBoard.Domain.Interfaces;

using DevBoard.Domain.Entities;

// out T = covariant. Lets you treat IReadOnlyRepository<Issue> as IReadOnlyRepository<BaseEntity>
// wherever only reading matters — this is what Jul 16's covariance lesson is teaching.
public interface IReadOnlyRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    IQueryable<T> Query();   // caller composes LINQ on top — nothing hits the DB yet
}