using FoodFirst.Dal.Enums;

namespace FoodFirst.Dto.Auth;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Phone);

public record LoginRequest(string Email, string Password);

public record StoreLoginRequest(Guid StoreId, string Pin);

public record AuthResponse(string Token, DateTime ExpiresAt, UserSummary User);

public record UserSummary(Guid Id, string Email, string FirstName, string LastName, UserRole Role);
