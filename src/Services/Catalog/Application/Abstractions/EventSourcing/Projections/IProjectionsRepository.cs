using System.Linq.Expressions;
using ECommerce.Abstractions.Messages.Queries.Paging;
using ECommerce.Abstractions.Projections;

namespace Application.Abstractions.EventSourcing.Projections;

public interface IProjectionsRepository
{
    Task<TProjection> FindAsync<TProjection>(Expression<Func<TProjection, bool>> predicate, CancellationToken cancellationToken)
        where TProjection : IProjection;

    Task<TProjection> GetAsync<TProjection, TId>(TId id, CancellationToken cancellationToken)
        where TProjection : IProjection
        where TId : struct;

    Task<IPagedResult<TProjection>> GetAllAsync<TProjection>(IPaging paging, Expression<Func<TProjection, bool>> predicate, CancellationToken cancellationToken)
        where TProjection : IProjection;

    Task UpsertAsync<TProjection>(TProjection replacement, CancellationToken cancellationToken)
        where TProjection : IProjection;

    Task UpsertManyAsync<TProjection>(IEnumerable<TProjection> replacements, CancellationToken cancellationToken)
        where TProjection : IProjection;

    Task DeleteAsync<TProjection>(Expression<Func<TProjection, bool>> filter, CancellationToken cancellationToken)
        where TProjection : IProjection;
    
    Task UpdateFieldAsync<TProjection, TField, TId>(TId id, Expression<Func<TProjection, TField>> field, TField value, CancellationToken cancellationToken)
        where TProjection : IProjection
        where TId : struct;
}