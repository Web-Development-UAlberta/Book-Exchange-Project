using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Exchange.Migrations
{
    /// <inheritdoc />
    public partial class SeedMessageAndNotifications : Migration
    {
        private static readonly Guid TestUserId = new("019dff3e-5156-7c3d-852b-1a1415f3368a");
        private static readonly Guid OtherUserId = new("019dff55-cc08-77a8-9b85-539869351661");

        private static readonly Guid Msg1 = new("eeeeeeee-0001-0000-0000-000000000001");
        private static readonly Guid Msg2 = new("eeeeeeee-0002-0000-0000-000000000002");
        private static readonly Guid Msg3 = new("eeeeeeee-0003-0000-0000-000000000003");
        private static readonly Guid Msg4 = new("eeeeeeee-0004-0000-0000-000000000004");
        private static readonly Guid Msg5 = new("eeeeeeee-0005-0000-0000-000000000005");

        private static readonly Guid Notif1 = new("ffffffff-0001-0000-0000-000000000001");
        private static readonly Guid Notif2 = new("ffffffff-0002-0000-0000-000000000002");
        private static readonly Guid Notif3 = new("ffffffff-0003-0000-0000-000000000003");
        private static readonly Guid Notif4 = new("ffffffff-0004-0000-0000-000000000004");
        private static readonly Guid Notif5 = new("ffffffff-0005-0000-0000-000000000005");
        private static readonly Guid Notif6 = new("ffffffff-0006-0000-0000-000000000006");
        private static readonly Guid Notif7 = new("ffffffff-0007-0000-0000-000000000007");

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // A short conversation thread between the two users.
            // No FK references to Listings / ExchangeRequests / Transactions yet

            migrationBuilder.Sql($"""
        INSERT INTO messages (id, sender_id, receiver_id, message_text, is_read, listing_id, exchange_request_id, transaction_id, created_at)
        VALUES
        ('{Msg1}', '{TestUserId}', '{OtherUserId}', 'Hi! Is your copy of A Tale of Two Cities still available?',  false, NULL, NULL, NULL, '2025-01-10 09:00:00+00'),
        ('{Msg2}', '{OtherUserId}', '{TestUserId}', 'Yes it is! Good condition, looking to swap for something by Jane Austen.', true,  NULL, NULL, NULL, '2025-01-10 09:15:00+00'),
        ('{Msg3}', '{TestUserId}', '{OtherUserId}', 'I have Pride and Prejudice — interested?',                              false, NULL, NULL, NULL, '2025-01-10 09:30:00+00'),
        ('{Msg4}', '{OtherUserId}', '{TestUserId}', 'That works great for me. Let''s set up the exchange!',                 true,  NULL, NULL, NULL, '2025-01-10 09:45:00+00'),
        ('{Msg5}', '{TestUserId}', '{OtherUserId}', 'Perfect. I''ll submit the exchange request now. Check your notifications!', false, NULL, NULL, NULL, '2025-01-10 10:00:00+00');
        """);

            migrationBuilder.Sql($"""
        INSERT INTO notifications (id, user_id, category, title, message, is_read, read_at, related_listing_id, related_exchange_request_id, related_transaction_id, created_at)
        VALUES
        ('{Notif1}', '{TestUserId}',  'match_found',        'Book Match Found',          'A book matching your wishlist is now available.',                    false, NULL,                    NULL, NULL, NULL, '2025-01-09 08:00:00+00'),
        ('{Notif2}', '{OtherUserId}', 'new_message',        'New Message',               'You have a new message from test@test.com.',                         true,  '2025-01-10 09:16:00+00', NULL, NULL, NULL, '2025-01-10 09:00:00+00'),
        ('{Notif3}', '{OtherUserId}', 'exchange_requested', 'New Exchange Request',      'test@test.com has requested an exchange for your listing.',          false, NULL,                    NULL, NULL, NULL, '2025-01-10 10:05:00+00'),
        ('{Notif4}', '{TestUserId}',  'exchange_accepted',  'Exchange Accepted',         'Your exchange request has been accepted by otheruser@test.com.',     true,  '2025-01-10 10:30:00+00', NULL, NULL, NULL, '2025-01-10 10:20:00+00'),
        ('{Notif5}', '{TestUserId}',  'transaction_update', 'Transaction Status Updated','Your transaction status has changed to Shipped.',                    false, NULL,                    NULL, NULL, NULL, '2025-01-12 14:00:00+00'),
        ('{Notif6}', '{OtherUserId}', 'wishlist_available', 'Wishlist Book Available',   'A book on your wishlist has been listed by another user.',           false, NULL,                    NULL, NULL, NULL, '2025-01-13 09:00:00+00'),
        ('{Notif7}', '{TestUserId}',  'exchange_rejected',  'Exchange Rejected',         'Your exchange request was declined.',                                true,  '2025-01-14 11:00:00+00', NULL, NULL, NULL, '2025-01-14 10:30:00+00');
        """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
        DELETE FROM notifications WHERE id IN ('{Notif1}','{Notif2}','{Notif3}','{Notif4}','{Notif5}','{Notif6}','{Notif7}');
        """);

            migrationBuilder.Sql($"""
        DELETE FROM messages WHERE id IN ('{Msg1}','{Msg2}','{Msg3}','{Msg4}','{Msg5}');
        """);
        }
    }
}