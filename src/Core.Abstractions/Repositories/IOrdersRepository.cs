using Core.Model.Entities;
using Core.Model.Enums;
using Core.Model.Filters;
using Core.Model.Pagination;

namespace Core.Abstractions.Repositories;

public interface IOrdersRepository
{
    Task<Order?> GetByIdAsync(long orderId, CancellationToken cancellationToken);

    Task<long> CreateAsync(string createdBy, DateTimeOffset createdAt, CancellationToken cancellationToken);

    Task<bool> UpdateStateAsync(long orderId, OrderState newState, CancellationToken cancellationToken);

    IAsyncEnumerable<Order> SearchAsync(OrderFilter filter, Paging paging, CancellationToken cancellationToken);
}