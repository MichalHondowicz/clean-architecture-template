using SharedKernel;

namespace IntegrationTests;

/// <summary>
/// A clock the test moves by hand. Register it on a host of its own (see the time convention in add-tests);
/// set <see cref="UtcNow"/> to values with <see cref="DateTimeKind.Utc"/>, as PostgreSQL requires.
/// </summary>
public sealed class TestDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; set; }
}
