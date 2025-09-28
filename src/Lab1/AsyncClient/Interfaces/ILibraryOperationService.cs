using Lab1.AsyncClient.Models;

namespace Lab1.AsyncClient.Interfaces;

public interface ILibraryOperationService
{
    void BeginOperation(Guid requestId, RequestModel model, CancellationToken cancellationToken);
}