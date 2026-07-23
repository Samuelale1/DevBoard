 using DevBoard.Domain.Entities;
 using System.Linq.Expressions;
 namespace DevBoard.Domain.Interfaces;
 
// Interface That works with any type of entity that inherits from 
// the <T>  stands for type parameter, which allows the interface to be used with different types of entities.
// the T can be seen as a placeholder for the actual entity type that will be used when implementing the interface.
// So T is what makes this interface a generic interface, 
// allowing it to be used with different types of entities that inherit from BaseEntity.
public interface IRepository <T> : IReadOnlyRepository<T> where T : BaseEntity // this is just saying it can only be used with classes, not datatypes like ints,strings,bool
{
    // Task just means this operation may take a while and will be done asynchronously, 
    // so it will return a Task object that represents the ongoing operation.
    // the question mark after T indicates that the method may return a null value if no entity with the specified ID is found. 
    // Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // IQueryable<T> Query();

// The GetAllAsync method retrieves all entities of type T from the repository asynchronously.
    //Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

}