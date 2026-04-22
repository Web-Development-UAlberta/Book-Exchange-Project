namespace Book_Exchange.Models;

public enum BookCondition
{
    LikeNew,
    VeryGood,
    Good,
    Acceptable,
    Poor
}

public enum ListingType
{
    Sell,
    Buy,
    Swap
}

public enum TransactionType
{
    BuySell,
    Swap,
    MultiSwap
}

public enum TransactionStatus
{
    Proposed,
    Negotiating,
    Confirmed,
    Shipped,
    Completed,
    Cancelled,
    Disputed
}

public enum ShippingStatus
{
    Pending,
    Quoted,
    LabelCreated,
    Shipped,
    Delivered,
    Cancelled
}

public enum NotificationType
{
    MatchFound,
    WishlistAvailable,
    NewMessage,
    OfferReceived,
    OfferAccepted,
    OfferRejected,
    TransactionUpdate
}

public enum NotificationStatus
{
    Unread,
    Read,
    Archived
}

public enum MessageType
{
    Text,
    Offer
}

public enum MessageStatus
{
    Sent,
    Read
}

public enum LocalityType
{
    Local,
    Provincial,
    National,
    International
}