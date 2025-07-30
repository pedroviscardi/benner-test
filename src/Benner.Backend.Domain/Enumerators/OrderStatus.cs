namespace Benner.Backend.Domain.Enumerators
{
    public enum OrderStatus
    {
        Draft = 1,
        Confirmed = 2,
        InProgress = 3,
        Delivered = 4,
        Cancelled = 5,
        Pending = 6
    }
}