using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.Refresh;

internal sealed class RefreshTokenCommandHandler(
    IApplicationDbContext context,
    ITokenProvider tokenProvider,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<RefreshTokenCommand, AccessTokensResponse>
{
    public async Task<Result<AccessTokensResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        RefreshToken? refreshToken = await context.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.Token == command.RefreshToken, cancellationToken);

        DateTime now = dateTimeProvider.UtcNow;

        if (refreshToken is null ||
            refreshToken.ExpiresOnUtc < now ||
            LoginExpiresOnUtc(refreshToken) < now)
        {
            return Result.Failure<AccessTokensResponse>(UserErrors.InvalidRefreshToken);
        }

        string accessToken = tokenProvider.Create(refreshToken.User);
        string newRefreshToken = tokenProvider.GenerateRefreshToken();

        // Rotate the refresh token so a stolen token can only be used once.
        // Each rotation extends the expiry, but never past the limit counted from the login.
        DateTime slidingExpiresOnUtc = now.AddDays(RefreshTokenExpirationInDays);
        DateTime loginExpiresOnUtc = LoginExpiresOnUtc(refreshToken);

        refreshToken.Token = newRefreshToken;
        refreshToken.ExpiresOnUtc = slidingExpiresOnUtc < loginExpiresOnUtc ? slidingExpiresOnUtc : loginExpiresOnUtc;

        await context.SaveChangesAsync(cancellationToken);

        return new AccessTokensResponse(accessToken, newRefreshToken);
    }

    private static DateTime LoginExpiresOnUtc(RefreshToken refreshToken) =>
        refreshToken.LoggedInOnUtc.AddDays(MaxDaysSinceLogin);

    private const int RefreshTokenExpirationInDays = 7;

    private const int MaxDaysSinceLogin = 30;
}
