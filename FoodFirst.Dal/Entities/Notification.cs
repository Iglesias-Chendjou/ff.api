using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public string? Data { get; set; }
    public DateTime SentAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
