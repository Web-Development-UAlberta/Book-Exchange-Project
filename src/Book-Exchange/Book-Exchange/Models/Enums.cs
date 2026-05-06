namespace Book_Exchange.Models;

using NpgsqlTypes;

public enum BookCondition
{
    [PgName("like_new")]
    LikeNew,

    [PgName("very_good")]
    VeryGood,

    [PgName("good")]
    Good,

    [PgName("acceptable")]
    Acceptable,

    [PgName("poor")]
    Poor
}

public enum ExchangeStatus
{
    [PgName("requested")]
    Requested,

    [PgName("accepted")]
    Accepted,

    [PgName("rejected")]
    Rejected,

    [PgName("cancelled")]
    Cancelled,

    [PgName("completed")]
    Completed
}

public enum TransactionStatus
{
    [PgName("confirmed")]
    Confirmed,

    [PgName("shipped")]
    Shipped,

    [PgName("completed")]
    Completed,

    [PgName("cancelled")]
    Cancelled,

    [PgName("disputed")]
    Disputed
}

public enum ShipmentStatus
{
    [PgName("pending")]
    Pending,

    [PgName("quoted")]
    Quoted,

    [PgName("label_created")]
    LabelCreated,

    [PgName("shipped")]
    Shipped,

    [PgName("delivered")]
    Delivered,

    [PgName("cancelled")]
    Cancelled
}

public enum NotificationCategory
{
    [PgName("match_found")]
    MatchFound,

    [PgName("wishlist_available")]
    WishlistAvailable,

    [PgName("new_message")]
    NewMessage,

    [PgName("exchange_requested")]
    ExchangeRequested,

    [PgName("exchange_accepted")]
    ExchangeAccepted,

    [PgName("exchange_rejected")]
    ExchangeRejected,

    [PgName("transaction_update")]
    TransactionUpdate
}