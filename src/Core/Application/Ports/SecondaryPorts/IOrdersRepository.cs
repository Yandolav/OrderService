using Core.Application.Filters;
using Core.Application.Pagination;
using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Application.Ports.SecondaryPorts;

public interface IOrdersRepository
{
    Task<Order?> GetByIdAsync(long orderId, ITransaction? transaction, CancellationToken cancellationToken);

    Task<long> CreateAsync(string createdBy, DateTimeOffset createdAt, ITransaction? transaction, CancellationToken cancellationToken);

    Task<bool> UpdateStateAsync(long orderId, OrderState newState, ITransaction? transaction, CancellationToken cancellationToken);

    IAsyncEnumerable<Order> SearchAsync(OrderFilter filter, Paging paging, ITransaction? transaction, CancellationToken cancellationToken);
}