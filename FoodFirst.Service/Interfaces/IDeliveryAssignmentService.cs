namespace FoodFirst.Service.Interfaces;

public interface IDeliveryAssignmentService
{
    Task<Guid?> AssignAsync(Guid orderId, CancellationToken ct = default);
}
