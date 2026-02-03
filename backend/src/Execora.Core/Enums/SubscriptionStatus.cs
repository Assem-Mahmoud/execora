namespace Execora.Core.Enums;

/// <summary>
/// Subscription status for tenant billing
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    /// Subscription is active and paid
    /// </summary>
    Active = 1,

    /// <summary>
    /// Subscription is suspended due to non-payment or violation
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// Trial period
    /// </summary>
    Trial = 3,

    /// <summary>
    /// Payment is past due
    /// </summary>
    PastDue = 4
}
