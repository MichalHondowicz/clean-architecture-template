using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SharedKernel;
using Web.Api.Infrastructure;

namespace IntegrationTests.Infrastructure;

public sealed class CustomResultsTests
{
    [Fact]
    public void Problem_Should_ReturnServerFailure_WhenErrorTypeIsFailure()
    {
        // Arrange
        var result = Result.Failure(Error.Failure("Test.Failure", "Failure description"));

        // Act
        IResult response = CustomResults.Problem(result);

        // Assert
        ProblemHttpResult problem = response.ShouldBeOfType<ProblemHttpResult>();
        problem.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        problem.ProblemDetails.Title.ShouldBe("Server failure");
        problem.ProblemDetails.Detail.ShouldBe("An unexpected error occurred");
    }
}
