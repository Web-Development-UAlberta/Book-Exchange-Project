namespace Book_Exchange.Tests.BackEnd;

using Book_Exchange.Models.DTOs.Book;
using Book_Exchange.Services.Interfaces;
using Moq;
using Xunit;

public class BookSearchUnitTests
{
    private readonly Mock<IBookSearchApi> _bookSearchApiMock;

    public BookSearchUnitTests()
    {
        _bookSearchApiMock = new Mock<IBookSearchApi>();
    }

    [Fact]
    public async Task UT_BOOK_01_SearchValidKeyword_ReturnsBooks()
    {
        // Arrange
        var searchText = "harry";

        _bookSearchApiMock
            .Setup(x => x.SearchBooksAsync(searchText, 10))
            .ReturnsAsync(new List<BookInfoDto>
            {
                new BookInfoDto
                {
                    Title = "Harry Potter",
                    Authors = new List<string> { "J. K. Rowling" },
                    Genres = new List<string> { "Fantasy" },
                    PublishedYear = 1997,
                    Publisher = "Bloomsbury",
                    Isbn13 = "9780439708180"
                }
            });

        // Act
        var result = await _bookSearchApiMock.Object.SearchBooksAsync(searchText, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Harry Potter", result[0].Title);
        Assert.Contains("J. K. Rowling", result[0].Authors);
    }

    [Fact]
    public async Task UT_BOOK_02_SearchEmptyKeyword_ReturnsEmptyList()
    {
        // Arrange
        _bookSearchApiMock
            .Setup(x => x.SearchBooksAsync("", 10))
            .ReturnsAsync(new List<BookInfoDto>());

        // Act
        var result = await _bookSearchApiMock.Object.SearchBooksAsync("", 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UT_BOOK_03_GetBookByValidIsbn_ReturnsBook()
    {
        // Arrange
        var isbn = "9780439708180";

        _bookSearchApiMock
            .Setup(x => x.GetBookByIsbnAsync(isbn))
            .ReturnsAsync(new BookInfoDto
            {
                Title = "Harry Potter",
                Authors = new List<string> { "J. K. Rowling" },
                PublishedYear = 1997,
                Isbn13 = isbn
            });

        // Act
        var result = await _bookSearchApiMock.Object.GetBookByIsbnAsync(isbn);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Harry Potter", result.Title);
        Assert.Equal(isbn, result.Isbn13);
    }

    [Fact]
    public async Task UT_BOOK_04_GetBookByInvalidIsbn_ReturnsNull()
    {
        // Arrange
        var isbn = "invalid-isbn";

        _bookSearchApiMock
            .Setup(x => x.GetBookByIsbnAsync(isbn))
            .ReturnsAsync((BookInfoDto?)null);

        // Act
        var result = await _bookSearchApiMock.Object.GetBookByIsbnAsync(isbn);

        // Assert
        Assert.Null(result);
    }
}
