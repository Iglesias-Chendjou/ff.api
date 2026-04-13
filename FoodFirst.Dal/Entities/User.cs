using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Address> Addresses { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public DeliveryPerson? DeliveryPerson { get; set; }
    public Subscription? Subscription { get; set; }
    public SurpriseBoxSubscription? SurpriseBoxSubscription { get; set; }
}
