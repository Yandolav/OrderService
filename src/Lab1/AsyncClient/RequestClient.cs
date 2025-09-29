using Lab1.AsyncClient.Interfaces;
using Lab1.AsyncClient.Models;
using System.Collections.Concurrent;

namespace Lab1.AsyncClient;

public sealed class RequestClient : IRequestClient, ILibraryOperationHandler
{
    private readonly ILibraryOperationService _service;
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<ResponseModel>> _pending = new ConcurrentDictionary<Guid, TaskCompletionSource<ResponseModel>>();

    public RequestClient(ILibraryOperationService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<ResponseModel> SendAsync(RequestModel request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return await Task.FromCanceled<ResponseModel>(cancellationToken);

        var requestId = Guid.NewGuid();

        var tcs = new TaskCompletionSource<ResponseModel>(TaskCreationOptions.RunContinuationsAsynchronously);

        _pending[requestId] = tcs;

        await using CancellationTokenRegistration registration = cancellationToken.Register(() =>
        {
            if (_pending.TryRemove(requestId, out TaskCompletionSource<ResponseModel>? removed))
            {
                removed.TrySetCanceled(cancellationToken);
            }
        });

        _service.BeginOperation(requestId, request, cancellationToken);

        return await tcs.Task.ConfigureAwait(false);
    }

    public void HandleOperationResult(Guid requestId, byte[] data)
    {
        if (_pending.TryRemove(requestId, out TaskCompletionSource<ResponseModel>? tcs))
        {
            tcs.TrySetResult(new ResponseModel(data));
        }
    }

    public void HandleOperationError(Guid requestId, Exception exception)
    {
        if (_pending.TryRemove(requestId, out TaskCompletionSource<ResponseModel>? tcs))
        {
            tcs.TrySetException(exception);
        }
    }
}