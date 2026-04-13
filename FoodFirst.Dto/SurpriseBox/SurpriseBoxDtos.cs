namespace FoodFirst.Dto.SurpriseBox;

public record SurpriseBoxPlanDto(Guid Id, string Name, string? Description, decimal MonthlyPrice, int DeliveriesPerMonth, decimal EstimatedBoxValue);

public record SubscribeSurpriseBoxRequest(Guid PlanId, Guid DeliveryAddressId);
