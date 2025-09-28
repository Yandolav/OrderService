using Lab1.AsyncClient.Models;

namespace Lab1.AsyncClient.Interfaces;

public interface IRequestClient
{
    Task<ResponseModel> SendAsync(RequestModel request, CancellationToken cancellationToken);
}