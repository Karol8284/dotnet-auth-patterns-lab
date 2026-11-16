
using Domain.Entities.JWT;
using Domain.Enums;

namespace UnitTests.Modules.Auth.JWT;

public class RefreshTokenTests
{
    private static readonly DateTime _now = new DateTime(2026, 11, 16, 9, 0, 0, DateTimeKind.Utc);
    [Fact]
    public void Revoke_Does_not_change_already_revoked_token()
    {
        var token = CreateRefreshToken(_now);
        var originalRevokedAt = _now.AddMinutes(1);
        var originalConcurrencyStamp = token.ConcurrencyStamp;

        token.Revoke(originalRevokedAt, RevocationReason.UserLogout);
        var concurrencyStampAfterRevoke = token.ConcurrencyStamp;

        var attemptedRevokedAt = _now.AddMinutes(2);
        Assert.Throws<InvalidOperationException>(
            () => token.Revoke(attemptedRevokedAt, RevocationReason.RefreshTokenReplay));

        Assert.Equal(originalRevokedAt, token.RevokedAt);
        Assert.Equal(RevocationReason.UserLogout, token.RevocationReason);
        Assert.NotEqual(originalConcurrencyStamp, concurrencyStampAfterRevoke);
        Assert.Equal(concurrencyStampAfterRevoke, token.ConcurrencyStamp);
    }

    [Fact]
    public void IsActive_Returns_true_for_unexpired_token()
    {
        var token = CreateRefreshToken(_now);
        Assert.True(token.IsActive(_now));
    }

    [Fact]
    public void IsActive_Returns_false_for_expired_token()
    {
        var token = CreateRefreshToken(_now);
        var expiredAt = _now.AddMinutes(11);
        Assert.False(token.IsActive(expiredAt));
    }

    [Fact]
    public void IsActive_Returns_false_for_revoked_token()
    {
        var token = CreateRefreshToken(_now);
        var revokedAt = _now.AddMinutes(5);
        token.Revoke(revokedAt, RevocationReason.UserLogout);
        Assert.False(token.IsActive(revokedAt));
    }

    [Fact]
    public void Revoke_Sets_timestamp_and_reason()
    {
        var token = CreateRefreshToken(_now);

        var revokedAt = _now.AddMinutes(1);

        token.Revoke(revokedAt, RevocationReason.UserLogout);

        Assert.Equal(revokedAt, token.RevokedAt);
        Assert.Equal(RevocationReason.UserLogout, token.RevocationReason);
    }

    [Fact]
    public void MarkAsRotated_Revokes_token_and_records_rotation_details()
    {
        var token = CreateRefreshToken(_now);
        var originalConcurrencyStamp = token.ConcurrencyStamp;
        var usedAt = _now.AddMinutes(1);
        var familyId = Guid.NewGuid();

        token.MarkAsRotated(usedAt, "203.0.113.10", "successor-hash", familyId);

        Assert.Equal(usedAt, token.RevokedAt);
        Assert.Equal(RevocationReason.TokenRotated, token.RevocationReason);
        Assert.Equal(usedAt, token.LastUsedAt);
        Assert.Equal("203.0.113.10", token.LastUsedByIp);
        Assert.Equal("successor-hash", token.ReplacedByTokenHash);
        Assert.Equal(familyId, token.FamilyId);
        Assert.NotEqual(originalConcurrencyStamp, token.ConcurrencyStamp);
    }

    [Fact]
    public void MarkAsRotated_Rejects_expired_token_without_changing_it()
    {
        var token = CreateRefreshToken(_now);
        var originalConcurrencyStamp = token.ConcurrencyStamp;

        Assert.Throws<InvalidOperationException>(
            () => token.MarkAsRotated(token.ExpiresAt, "203.0.113.10", "successor-hash", Guid.NewGuid()));

        Assert.Null(token.RevokedAt);
        Assert.Null(token.RevocationReason);
        Assert.Null(token.LastUsedAt);
        Assert.Null(token.LastUsedByIp);
        Assert.Null(token.ReplacedByTokenHash);
        Assert.Equal(originalConcurrencyStamp, token.ConcurrencyStamp);
    }

    [Fact]
    public void IsActive_Returns_false_when_expiration_time_is_reached()
    {
        var token = CreateRefreshToken(_now);

        Assert.False(token.IsActive(token.ExpiresAt.AddMinutes(11)));
    }

    private static RefreshToken CreateRefreshToken(DateTime now)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            UserEmail = "example@example.com",
            UserDisplayName = "Example User",
            UserRole = UserRole.User,
            IsEmailConfirmed = true,
            TokenHash = $"hashed_{Guid.NewGuid():N}",
            ConcurrencyStamp = Guid.NewGuid().ToString("N"),
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(10),
            LastUsedAt = null,
            LastUsedByIp = null,
            RevokedAt = null,
            RevocationReason = null,
            ReplacedByTokenHash = null,
            FamilyId = Guid.NewGuid()
        };
    }
}
