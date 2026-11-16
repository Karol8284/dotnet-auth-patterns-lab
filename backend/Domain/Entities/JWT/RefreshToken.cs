using Domain.Enums;

namespace Domain.Entities.JWT
{
    /// <summary>
    /// Persisted refresh token with user snapshot for rotation.
    /// Long live span (e.g. 7 days) and stored securely in database.
    /// </summary>
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string UserEmail { get; set; } = string.Empty;

        public string UserDisplayName { get; set; } = string.Empty;

        public UserRole UserRole { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public string TokenHash { get; set; } = string.Empty;

        public string ConcurrencyStamp { get; set; } = string.Empty;

        public string? DeviceName { get; private set; }
        public string? UserAgent { get; private set; }

        /// <summary>
        /// IP address from which the token was created (for audit trail).
        /// </summary>
        public string CreatedByIp { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// When the token was last used (null if never used).
        /// </summary>
        public DateTime? LastUsedAt { get; set; }

        /// <summary>
        /// IP address from which the token was last used (null if never used).
        /// Helps detect anomalies if token is used from different IP.
        /// </summary>
        public string? LastUsedByIp { get; set; }

        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// Reason for token revocation (null if token is still active).
        /// </summary>
        public RevocationReason? RevocationReason { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        /// <summary>
        /// Family ID for token rotation chain. Groups related tokens together.
        /// Null for standalone tokens or legacy tokens without family tracking.
        /// </summary>
        public Guid? FamilyId { get; set; }

        public bool IsActive(DateTime now)
        {
            return !RevokedAt.HasValue && ExpiresAt > now;
        }

        public void Revoke(DateTime now, RevocationReason reason, string? replacedByTokenHash = null)
        {
            if (RevokedAt.HasValue)
            {
                throw new InvalidOperationException("Token is already revoked.");
            }
            RevokedAt = now;
            RevocationReason = reason;
            ReplacedByTokenHash = replacedByTokenHash;
            ConcurrencyStamp = GenerateConcurrencyStamp();
        }

        public void MarkAsRotated(DateTime usedAt, string usedByIp, string replacedByTokenHash, Guid familyId)
        {
            if (!IsActive(usedAt))
            {
                throw new InvalidOperationException("Token is not active and cannot be rotated.");
            }

            ArgumentException.ThrowIfNullOrEmpty(replacedByTokenHash, nameof(replacedByTokenHash));

            if (familyId == Guid.Empty)
            {
                throw new ArgumentException("Family ID cannot be empty.", nameof(familyId));
            }

            Revoke(usedAt, Domain.Enums.RevocationReason.TokenRotated, replacedByTokenHash);
            LastUsedAt = usedAt;
            LastUsedByIp = usedByIp;
            FamilyId = familyId;
        }

        private static string GenerateConcurrencyStamp()
            => Guid.NewGuid().ToString("N");
    }
}
