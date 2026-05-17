# Customer Request

A book lover has a vision for a platform where users can exchange books.

Users would be able to list the books they currently own and the books they want to acquire. The system would then match users based on their book lists, using mock data to identify and match the titles users enter.

But here's the catch: users should have the option to sell their books for cash, purchase books for a set price, or propose swaps with other users. When suggesting swaps, the system would need to consider any value discrepancies between the books being exchanged (e.g., suggesting a 3x$10 book swap for a 1x$30 book). Users should also be able to filter and search for books based on criteria like author, genre, publishing date, and price range.

### User Profiles and Ratings:

- Implement user profiles that display a user's book collection, wishlist, and ratings/reviews for books they've read. Allow users to rate and review books they've acquired through the platform.

### Book Condition Management:

- Enable users to specify the condition of the books they're listing for sale or swap, and provide guidelines or a rating system for book conditions.

### Shipping and Delivery Integration:

- Integrate with shipping carriers and provide shipping cost calculations based on the user's location and the book's weight/dimensions. Automate shipping labels.

### Wish List and Notification System:

- Allow users to create and manage a wish list of desired books, and implement a notification system that alerts users when a desired book becomes available for swap or purchase.

Business Rules/Logic/Context:

1. The system should have a database to store user information, book listings, and desired book lists.
2. The system should integrate with mock data to retrieve book information based on user input (title, author, ISBN, etc.).
3. Users should be able to create an account and list the books they currently own and the books they want to acquire.
4. For each book listing, users should be able to specify whether they want to sell (with a cash price), swap, or purchase (with an offer price).
5. The system should match users based on their book lists, considering both swap requests and buy/sell requests.
6. When forming swap suggestions, the system should take into account value discrepancies between the books being swapped and suggest combinations that balance the values (e.g., 3x$10 books for 1x$30 book).
7. The system should provide advanced search and filtering options for users to find specific books based on author, genre, publishing date, and price range.
8. The system should have a messaging system or notification system to facilitate communication between users for arranging book exchanges or purchases.
9. The system should have a rating and review system for users to provide feedback on their book exchange experiences.

Additions:
● Allow up to 3 permutations of counter-offer for each back-and-forth.
