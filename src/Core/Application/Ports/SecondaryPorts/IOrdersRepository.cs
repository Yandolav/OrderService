using Domain.Entities;
using Domain.Entities.Filters;
using Domain.Entities.Pagination;
using Domain.Enums;

namespace Domain.Repositories;

public interface IOrdersRepository
{
    Task<Order?> GetByIdAsync(long orderId, ITransaction? transaction, CancellationToken cancellationToken);

    Task<long> CreateAsync(string createdBy, DateTimeOffset createdAt, ITransaction? transaction, CancellationToken cancellationToken);

    Task<bool> UpdateStateAsync(long orderId, OrderState newState, ITransaction? transaction, CancellationToken cancellationToken);

    IAsyncEnumerable<Order> SearchAsync(OrderFilter filter, Paging paging, ITransaction? transaction, CancellationToken cancellationToken);
}