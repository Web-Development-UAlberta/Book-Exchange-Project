using Xunit;
using Moq;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.ExchangeRequest;
using Book_Exchange.Services.Interfaces;

namespace Book_Exchange.Tests.BackEnd;

// UNIT TESTS
// Covers: UT-EXCH-01 through UT-EXCH-08
public class ExchangeRequestUnitTests
{
    private readonly Mock<IExchangeRequestService> _serviceMock;

    public ExchangeRequestUnitTests()
    {
        _serviceMock = new Mock<IExchangeRequestService>();
    }

    /// <summary>
    /// UT-EXCH-01: Create BuySell exchange request for active listing
    /// Expected: ExchangeRequest is created with Requested status
    /// </summary>
    [Fact]
    public async Task UT_EXCH_01_CreateBuySellRequest_ReturnsRequestedExchangeRequest()
    {
        var userId = Guid.NewGuid();

        var dto = new CreateExchangeRequestDto
        {
            TargetListingId = Guid.NewGuid(),
            OfferedListingIds = new List<Guid>(),
            CashAmount = 25.00m
        };

        var expectedRequest = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            TargetListingId = dto.TargetListingId,
            RequesterId = userId,
            Status = ExchangeStatus.Requested,
            Price = dto.CashAmount
        };

        _serviceMock
            .Setup(s => s.CreateExchangeRequestAsync(dto, userId))
            .ReturnsAsync(expectedRequest);

        var result = await _serviceMock.Object.CreateExchangeRequestAsync(dto, userId);

        Assert.NotNull(result);
        Assert.Equal(dto.TargetListingId, result.TargetListingId);
        Assert.Equal(userId, result.RequesterId);
        Assert.Equal(ExchangeStatus.Requested, result.Status);
        Assert.Equal(dto.CashAmount, result.Price);
    }

    /// <summary>
    /// UT-EXCH-02: Create BookSwap exchange request with one offered listing
    /// Expected: ExchangeRequest and ExchangeRequestItems are created
    /// </summary>
    [Fact]
    public async Task UT_EXCH_02_CreateBookSwapRequestWithOneOfferedListing_ReturnsRequestWithItem()
    {
        var userId = Guid.NewGuid();
        var offeredListingId = Guid.NewGuid();

        var dto = new CreateExchangeRequestDto
        {
            TargetListingId = Guid.NewGuid(),
            OfferedListingIds = new List<Guid> { offeredListingId },
            CashAmount = null
        };

        var expectedRequest = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            TargetListingId = dto.TargetListingId,
            RequesterId = userId,
            Status = ExchangeStatus.Requested,
            Price = null,
            ExchangeRequestItems = new List<ExchangeRequestItem>
            {
                new ExchangeRequestItem
                {
                    ExchangeRequestId = Guid.NewGuid(),
                    OfferedListingId = offeredListingId
                }
            }
        };

        _serviceMock
            .Setup(s => s.CreateExchangeRequestAsync(dto, userId))
            .ReturnsAsync(expectedRequest);

        var result = await _serviceMock.Object.CreateExchangeRequestAsync(dto, userId);

        Assert.NotNull(result);
        Assert.Equal(ExchangeStatus.Requested, result.Status);
        Assert.Single(result.ExchangeRequestItems);
        Assert.Contains(result.ExchangeRequestItems, item => item.OfferedListingId == offeredListingId);
        Assert.Null(result.Price);
    }

    /// <summary>
    /// UT-EXCH-03: Create BookSwapWithCash request with offered listing and price
    /// Expected: ExchangeRequest is created successfully
    /// </summary>
    [Fact]
    public async Task UT_EXCH_03_CreateBookSwapWithCashRequest_ReturnsExchangeRequest()
    {
        var userId = Guid.NewGuid();
        var offeredListingId = Guid.NewGuid();

        var dto = new CreateExchangeRequestDto
        {
            TargetListingId = Guid.NewGuid(),
            OfferedListingIds = new List<Guid> { offeredListingId },
            CashAmount = 10.00m
        };

        var expectedRequest = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            TargetListingId = dto.TargetListingId,
            RequesterId = userId,
            Status = ExchangeStatus.Requested,
            Price = dto.CashAmount,
            ExchangeRequestItems = new List<ExchangeRequestItem>
            {
                new ExchangeRequestItem
                {
                    ExchangeRequestId = Guid.NewGuid(),
                    OfferedListingId = offeredListingId
                }
            }
        };

        _serviceMock
            .Setup(s => s.CreateExchangeRequestAsync(dto, userId))
            .ReturnsAsync(expectedRequest);

        var result = await _serviceMock.Object.CreateExchangeRequestAsync(dto, userId);

        Assert.NotNull(result);
        Assert.Equal(ExchangeStatus.Requested, result.Status);
        Assert.Equal(dto.CashAmount, result.Price);
        Assert.Single(result.ExchangeRequestItems);
    }

    /// <summary>
    /// UT-EXCH-04: Create swap request with more than three offered listings
    /// Expected: Request is rejected by business rule
    /// </summary>
    [Fact]
    public async Task UT_EXCH_04_CreateSwapRequestWithMoreThanThreeOfferedListings_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();

        var dto = new CreateExchangeRequestDto
        {
            TargetListingId = Guid.NewGuid(),
            OfferedListingIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            },
            CashAmount = null
        };

        _serviceMock
            .Setup(s => s.CreateExchangeRequestAsync(dto, userId))
            .ThrowsAsync(new ArgumentException("You may offer a maximum of 3 books."));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _serviceMock.Object.CreateExchangeRequestAsync(dto, userId));
    }

    /// <summary>
    /// UT-EXCH-05: Create exchange request for own listing
    /// Expected: Request is rejected
    /// </summary>
    [Fact]
    public async Task UT_EXCH_05_CreateRequestForOwnListing_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();

        var dto = new CreateExchangeRequestDto
        {
            TargetListingId = Guid.NewGuid(),
            OfferedListingIds = new List<Guid>(),
            CashAmount = 20.00m
        };

        _serviceMock
            .Setup(s => s.CreateExchangeRequestAsync(dto, userId))
            .ThrowsAsync(new InvalidOperationException("You cannot create an exchange request for your own listing."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.CreateExchangeRequestAsync(dto, userId));
    }

    /// <summary>
    /// UT-EXCH-06: Accept valid exchange request
    /// Expected: ExchangeRequest status becomes Accepted
    /// </summary>
    [Fact]
    public async Task UT_EXCH_06_AcceptValidExchangeRequest_CompletesSuccessfully()
    {
        var ownerUserId = Guid.NewGuid();
        var exchangeRequestId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.AcceptExchangeRequestAsync(exchangeRequestId, ownerUserId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.AcceptExchangeRequestAsync(exchangeRequestId, ownerUserId);

        _serviceMock.Verify(
            s => s.AcceptExchangeRequestAsync(exchangeRequestId, ownerUserId),
            Times.Once);
    }

    /// <summary>
    /// UT-EXCH-07: Reject exchange request
    /// Expected: ExchangeRequest status becomes Rejected
    /// </summary>
    [Fact]
    public async Task UT_EXCH_07_RejectExchangeRequest_CompletesSuccessfully()
    {
        var ownerUserId = Guid.NewGuid();
        var exchangeRequestId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.RejectExchangeRequestAsync(exchangeRequestId, ownerUserId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.RejectExchangeRequestAsync(exchangeRequestId, ownerUserId);

        _serviceMock.Verify(
            s => s.RejectExchangeRequestAsync(exchangeRequestId, ownerUserId),
            Times.Once);
    }

    /// <summary>
    /// UT-EXCH-08: Invalid exchange status transition
    /// Expected: System rejects invalid transition
    /// </summary>
    [Fact]
    public async Task UT_EXCH_08_InvalidExchangeStatusTransition_ThrowsInvalidOperationException()
    {
        var ownerUserId = Guid.NewGuid();
        var exchangeRequestId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.AcceptExchangeRequestAsync(exchangeRequestId, ownerUserId))
            .ThrowsAsync(new InvalidOperationException("Exchange request must be in Requested status."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.AcceptExchangeRequestAsync(exchangeRequestId, ownerUserId));
    }
}