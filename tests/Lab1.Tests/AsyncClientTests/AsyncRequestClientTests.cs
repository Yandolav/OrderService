using FluentAssertions;
using Lab1.AsyncClient;
using Lab1.AsyncClient.Interfaces;
using Lab1.AsyncClient.Models;
using NSubstitute;
using Xunit;

namespace Lab1.Tests.AsyncClientTests;

public class AsyncRequestClientTests
{
    [Fact]
    public async Task Scenario1_Result_Comes_Later()
    {
        // arrange
        ILibraryOperationService svc = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(svc);

        Guid captured = Guid.Empty;
        svc.When(s => s.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(ci => { captured = (Guid)ci[0]; });

        CancellationToken ct = CancellationToken.None;
        Task<ResponseModel> task = client.SendAsync(MakeRequest(), ct);

        // act
        await Task.Delay(10);
        client.HandleOperationResult(captured, new byte[] { 7, 8, 9 });

        // assert
        ResponseModel result = await task;
        result.Data.Should().BeEquivalentTo(new byte[] { 7, 8, 9 });
    }

    [Fact]
    public async Task Scenario2_Error_Comes_Later()
    {
        // arrange
        ILibraryOperationService svc = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(svc);

        Guid captured = Guid.Empty;
        svc.When(s => s.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(ci => { captured = (Guid)ci[0]; });

        CancellationToken ct = CancellationToken.None;
        Task<ResponseModel> task = client.SendAsync(MakeRequest(), ct);

        // act
        await Task.Delay(10);
        Exception ex = new InvalidOperationException("Mya");
        client.HandleOperationError(captured, ex);

        // assert
        Func<Task> act = async () => await task;
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Mya");
    }

    [Fact]
    public async Task Scenario3_AlreadyCanceledToken()
    {
        // arrange
        ILibraryOperationService svc = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(svc);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // act
        Task<ResponseModel> task = client.SendAsync(MakeRequest(), cts.Token);

        // assert
        Func<Task> act = async () => await task;
        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task Scenario4_Cancel_After_Start()
    {
        // arrange
        ILibraryOperationService svc = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(svc);

        var cts = new CancellationTokenSource();
        Task<ResponseModel> task = client.SendAsync(MakeRequest(), cts.Token);

        // act
        await Task.Delay(10);
        cts.Cancel();

        // assert
        Func<Task> act = async () => await task;
        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task Scenario5_Result_Synchronously_Inside_BeginOperation()
    {
        // arrange
        ILibraryOperationService svc = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(svc);

        byte[] expected = new byte[] { 4, 5, 6 };

        svc.When(s => s.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(ci =>
            {
                var id = (Guid)ci[0];
                client.HandleOperationResult(id, expected);
            });

        // act
        Task<ResponseModel> task = client.SendAsync(MakeRequest(), CancellationToken.None);
        ResponseModel result = await task;

        // assert
        result.Data.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Scenario6_Erroфr_Synchronously_Inside_BeginOperation()
    {
        // arrange
        ILibraryOperationService svc = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(svc);

        var boom = new ApplicationException("Mya");

        svc.When(s => s.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(ci =>
            {
                var id = (Guid)ci[0];
                client.HandleOperationError(id, boom);
            });

        // act
        Task<ResponseModel> task = client.SendAsync(MakeRequest(), CancellationToken.None);

        // assert
        Func<Task> act = async () => await task;
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Mya");
    }

    [Fact]
    public async Task Scenario7_Cancel_Synchronously_Inside_BeginOperation()
    {
        // arrange
        ILibraryOperationService svc = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(svc);

        var cts = new CancellationTokenSource();

        svc.When(s => s.BeginOperation(Arg.Any<Guid>(), Arg.Any<RequestModel>(), Arg.Any<CancellationToken>()))
            .Do(ci =>
            {
                cts.Cancel();
            });

        // act
        Task<ResponseModel> task = client.SendAsync(MakeRequest(), cts.Token);

        // assert
        Func<Task> act = async () => await task;
        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    private static RequestModel MakeRequest()
    {
        return new RequestModel("op", new byte[] { 1, 2, 3 });
    }
}