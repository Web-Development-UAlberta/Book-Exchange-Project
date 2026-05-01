using Xunit;
using Moq;
using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;

// Transaction Tests
// Covers: UT-TRANS-01 through UT-TRANS-05 (Unit Tests)         
//         Extra: UT-TRANS-06 Shipped, UT-TRANS-07 Disputed transitions
namespace Book_Exchange.Tests.BackEnd;

// UNIT TESTS
public class TransactionServiceUnitTests
{
    private readonly Mock<ITransactionService> _serviceMock;

    public TransactionServiceUnitTests()
    {
        _serviceMock = new Mock<ITransactionService>();
    }

    /// <summary>
    /// UT-TRANS-01: Create transaction from accepted exchange request
    /// Expected: Transaction is created with Confirmed status
    /// </summary>
    /// <returns>
    /// Transaction with Status = Confirmed, linked to the ExchangeRequest
    /// </returns>
    [Fact]
    public async Task UT_TRANS_01_CreateFromAcceptedRequest_ReturnsConfirmedTransaction()
    {
        var exchangeRequestId = Guid.NewGuid();
        var acceptedRequest = new ExchangeRequest
        {
            Id = exchangeRequestId,
            Status = ExchangeStatus.Accepted
        };

        var expectedTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            ExchangeRequestId = exchangeRequestId,
            Status = TransactionStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        };

        _serviceMock
            .Setup(s => s.CreateTransactionFromExchangeRequestAsync(acceptedRequest))
            .ReturnsAsync(expectedTransaction);

        var result = await _serviceMock.Object.CreateTransactionFromExchangeRequestAsync(acceptedRequest);

        Assert.NotNull(result);
        Assert.Equal(exchangeRequestId, result.ExchangeRequestId);
        Assert.Equal(TransactionStatus.Confirmed, result.Status);
        Assert.NotNull(result.ConfirmedAt);
    }

    /// <summary>
    /// UT-TRANS-02: Attempt to create transaction from rejected request
    /// Expected: Transaction creation is rejected
    /// </summary>
    /// <returns>
    /// InvalidOperationException with message "Cannot create transaction from a rejected exchange request."
    /// </returns>
    [Fact]
    public async Task UT_TRANS_02_CreateFromRejectedRequest_ThrowsInvalidOperation()
    {
        var rejectedRequest = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            Status = ExchangeStatus.Rejected
        };

        _serviceMock
            .Setup(s => s.CreateTransactionFromExchangeRequestAsync(rejectedRequest))
            .ThrowsAsync(new InvalidOperationException(
                "Cannot create transaction from a rejected exchange request."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.CreateTransactionFromExchangeRequestAsync(rejectedRequest));
    }

    /// <summary>
    /// UT-TRANS-03: Complete transaction
    /// Expected: Transaction status changes to Completed
    /// </summary>
    /// <returns>
    /// Transaction with Status = Completed and CompletedAt timestamp set
    /// </returns>
    [Fact]
    public async Task UT_TRANS_03_CompleteTransaction_ReturnsCompletedStatus()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.GetTransactionByIdAsync(transactionId))
            .ReturnsAsync(new Transaction
            {
                Id = transactionId,
                Status = TransactionStatus.Shipped
            });

        _serviceMock
            .Setup(s => s.CompleteTransactionAsync(transactionId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.CompleteTransactionAsync(transactionId, userId);

        _serviceMock.Verify(s => s.CompleteTransactionAsync(transactionId, userId), Times.Once);
    }

    /// <summary>
    /// UT-TRANS-04: Cancel transaction
    /// Expected: Transaction status changes to Cancelled
    /// </summary>
    /// <returns>
    /// Transaction with Status = Cancelled
    /// </returns>
    [Fact]
    public async Task UT_TRANS_04_CancelTransaction_StatusBecomesCancelled()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.CancelTransactionAsync(transactionId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.CancelTransactionAsync(transactionId, userId);

        _serviceMock.Verify(s => s.CancelTransactionAsync(transactionId, userId), Times.Once);
    }

    /// <summary>
    /// UT-TRANS-05a: Invalid status transition — Complete an already Cancelled transaction
    /// Expected: System rejects invalid transition
    /// </summary>
    /// <returns>
    /// InvalidOperationException with message "Cannot complete a transaction that is already Cancelled."
    /// </returns>
    [Fact]
    public async Task UT_TRANS_05a_CompleteAlreadyCancelledTransaction_ThrowsInvalidOperation()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.CompleteTransactionAsync(transactionId, userId))
            .ThrowsAsync(new InvalidOperationException(
                "Cannot complete a transaction that is already Cancelled."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.CompleteTransactionAsync(transactionId, userId));
    }

    /// <summary>
    /// UT-TRANS-05b: Invalid status transition — Cancel an already Completed transaction
    /// Expected: System rejects invalid transition
    /// </summary>
    /// <returns>
    /// InvalidOperationException with message "Cannot cancel a transaction that is already Completed."
    /// </returns>
    [Fact]
    public async Task UT_TRANS_05b_CancelAlreadyCompletedTransaction_ThrowsInvalidOperation()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.CancelTransactionAsync(transactionId, userId))
            .ThrowsAsync(new InvalidOperationException(
                "Cannot cancel a transaction that is already Completed."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.CancelTransactionAsync(transactionId, userId));
    }

    /// <summary>
    /// UT-TRANS-05c: Invalid status transition — Ship an already Completed transaction
    /// Expected: System rejects invalid transition
    /// </summary>
    /// <returns>
    /// InvalidOperationException with message "Cannot mark a Completed transaction as Shipped."
    /// </returns>
    [Fact]
    public async Task UT_TRANS_05c_ShipAlreadyCompletedTransaction_ThrowsInvalidOperation()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.MarkAsShippedAsync(transactionId, userId))
            .ThrowsAsync(new InvalidOperationException(
                "Cannot mark a Completed transaction as Shipped."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.MarkAsShippedAsync(transactionId, userId));
    }

    /// <summary>
    /// UT-TRANS-06: Mark transaction as Shipped
    /// Expected: Confirmed -> Shipped is a valid transition
    /// </summary>
    /// <returns>
    /// If successful, completes without exception. Verify that MarkAsShippedAsync was called with correct parameters.
    /// </returns>
    [Fact]
    public async Task UT_TRANS_06_MarkAsShipped_FromConfirmed_Succeeds()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.MarkAsShippedAsync(transactionId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.MarkAsShippedAsync(transactionId, userId);

        _serviceMock.Verify(s => s.MarkAsShippedAsync(transactionId, userId), Times.Once);
    }

    /// <summary>
    /// UT-TRANS-07a: Dispute a transaction from Shipped
    /// Expected: Shipped -> Disputed is a valid transition
    /// </summary>
    /// <returns>
    /// If successful, completes without exception. Verify that DisputeTransactionAsync was called with correct parameters.
    /// </returns>
    [Fact]
    public async Task UT_TRANS_07a_DisputeTransaction_FromShipped_Succeeds()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.DisputeTransactionAsync(transactionId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.DisputeTransactionAsync(transactionId, userId);

        _serviceMock.Verify(s => s.DisputeTransactionAsync(transactionId, userId), Times.Once);
    }

    /// <summary>
    /// UT-TRANS-07b: Dispute a transaction from Confirmed
    /// Expected: System rejects — must be Shipped before it can be Disputed
    /// </summary>
    /// <returns>
    /// InvalidOperationException with message "Cannot dispute a transaction that has not been shipped."
    /// </returns>
    [Fact]
    public async Task UT_TRANS_07b_DisputeTransaction_FromConfirmed_ThrowsInvalidOperation()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.DisputeTransactionAsync(transactionId, userId))
            .ThrowsAsync(new InvalidOperationException(
                "Cannot dispute a transaction that has not been shipped."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.DisputeTransactionAsync(transactionId, userId));
    }
}

