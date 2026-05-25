using HotelBooking.Core.DTOs.Auth;

namespace HotelBooking.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task LogoutAsync(string userId, string refreshToken);
    Task<UserInfoDto> GetProfileAsync(string userId);
}
