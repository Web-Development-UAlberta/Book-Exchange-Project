namespace Book_Exchange.Tests.Integration;

using Book_Exchange.Controllers;
using Book_Exchange.Models.DTOs.Book;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class BookSearchControllerTests
{
    [Fact]
    public async Task IT_BOOK_01_Index_WithSearchText_ReturnsBooks()
    {
        // Arrange
        var mockApi = new Mock<IBookSearchApi>();

        mockApi.Setup(x => x.SearchBooksAsync("harry", 10))
            .ReturnsAsync(new List<BookInfoDto>
            {
                new BookInfoDto
                {
                    Title = "Harry Potter",
                    Authors = new List<string> { "J. K. Rowling" }
                }
            });

        var controller = new BookSearchController(mockApi.Object);

        // Act
        var result = await controller.Search("harry");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<BookInfoDto>>(viewResult.Model);

        Assert.Single(model);
        Assert.Equal("Harry Potter", model[0].Title);
    }

    [Fact]
    public async Task IT_BOOK_02_Index_WithEmptySearchText_ReturnsEmptyList()
    {
        // Arrange
        var mockApi = new Mock<IBookSearchApi>();
        var controller = new BookSearchController(mockApi.Object);

        // Act
        var result = await controller.Search("");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<BookInfoDto>>(viewResult.Model);

        Assert.Empty(model);
    }
}
