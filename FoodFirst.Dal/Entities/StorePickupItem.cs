namespace FoodFirst.Dal.Entities;

public class StorePickupItem
{
    public Guid Id { get; set; }
    public Guid StorePickupId { get; set; }
    public Guid StoreInventoryId { get; set; }
    public int ExpectedQuantity { get; set; }
    public int CollectedQuantity { get; set; }
    public bool IsConform { get; set; } = true;
    public string? NonConformityReason { get; set; }

    public StorePickup StorePickup { get; set; } = null!;
    public StoreInventory StoreInventory { get; set; } = null!;
}
