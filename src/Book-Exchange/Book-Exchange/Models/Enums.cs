namespace Book_Exchange.Models;

public enum BookCondition
{
    LikeNew,
    VeryGood,
    Good,
    Acceptable,
    Poor
}

public enum ExchangeStatus
{
    Requested,
    Accepted,
    Rejected,
    Cancelled,
    Completed
}

public enum TransactionStatus
{
    Confirmed,
    Shipped,
    Completed,
    Cancelled,
    Disputed
}

public enum ShipmentStatus
{
    Pending,
    Quoted,
    LabelCreated,
    Shipped,
    Delivered,
    Cancelled
}

public enum NotificationCategory
{
    MatchFound,
    WishlistAvailable,
    NewMessage,
    ExchangeRequested,
    ExchangeAccepted,
    ExchangeRejected,
    TransactionUpdate
}
