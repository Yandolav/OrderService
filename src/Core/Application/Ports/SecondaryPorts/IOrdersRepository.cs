using Core.Application.Filters;
using Core.Application.Pagination;
using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Application.Ports.SecondaryPorts;

public interface IOrdersRepository
{
    Task<Order?> GetByIdAsync(long orderId, CancellationToken cancellationToken);

    Task<long> CreateAsync(string createdBy, DateTimeOffset createdAt, CancellationToken cancellationToken);

    Task<bool> UpdateStateAsync(long orderId, OrderState newState, CancellationToken cancellationToken);

    IAsyncEnumerable<Order> SearchAsync(OrderFilter filter, Paging paging, CancellationToken cancellationToken);
}