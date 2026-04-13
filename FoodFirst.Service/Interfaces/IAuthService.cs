using FoodFirst.Dto.Auth;

namespace FoodFirst.Service.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResponse> StoreLoginAsync(StoreLoginRequest request, CancellationToken ct = default);
}
