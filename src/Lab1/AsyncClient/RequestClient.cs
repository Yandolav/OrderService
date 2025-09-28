using Lab1.AsyncClient.Interfaces;
using Lab1.AsyncClient.Models;
using System.Collections.Concurrent;

namespace Lab1.AsyncClient;

public sealed class RequestClient : IRequestClient, ILibraryOperationHandler
{
    private readonly ILibraryOperationService _service;
    private readonly ConcurrentDictionary<Guid, Pending> _pending = new ConcurrentDictionary<Guid, Pending>();

    private sealed class Pending
    {
        public TaskCompletionSource<ResponseModel> Tcs { get; }

        public CancellationTokenRegistration Registration { get; set; }

        public Pending(TaskCompletionSource<ResponseModel> tcs)
        {
            Tcs = tcs;
            Registration = default;
        }

        public void DisposeRegistration()
        {
            Registration.Dispose();
        }
    }

    public RequestClient(ILibraryOperationService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public Task<ResponseModel> SendAsync(RequestModel request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<ResponseModel>(cancellationToken);

        var requestId = Guid.NewGuid();

        var tcs = new TaskCompletionSource<ResponseModel>(TaskCreationOptions.RunContinuationsAsynchronously);

        var pending = new Pending(tcs);

        _pending[requestId] = pending;

        pending.Registration = cancellationToken.Register(() =>
        {
            if (_pending.TryRemove(requestId, out Pending? removed))
            {
                removed.Tcs.TrySetCanceled(cancellationToken);
                removed.DisposeRegistration();
            }
            else
            {
                tcs.TrySetCanceled(cancellationToken);
            }
        });

        _service.BeginOperation(requestId, request, cancellationToken);

        return tcs.Task;
    }

    public void HandleOperationResult(Guid requestId, byte[] data)
    {
        if (_pending.TryRemove(requestId, out Pending? pending))
        {
            try
            {
                var response = new ResponseModel(data);
                pending.Tcs.TrySetResult(response);
            }
            finally
            {
                pending.DisposeRegistration();
            }
        }
    }

    public void HandleOperationError(Guid requestId, Exception exception)
    {
        if (_pending.TryRemove(requestId, out Pending? pending))
        {
            try
            {
                pending.Tcs.TrySetException(exception);
            }
            finally
            {
                pending.DisposeRegistration();
            }
        }
    }
}