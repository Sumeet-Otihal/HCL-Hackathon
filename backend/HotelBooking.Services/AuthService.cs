using System.Security.Cryptography;
using HotelBooking.Core.DTOs.Auth;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Exceptions;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.Helpers.Settings;
using HotelBooking.Core.Interfaces.Repositories;
using HotelBooking.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace HotelBooking.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly IGenericRepository<RefreshToken> _refreshTokenRepo;

    public AuthService(
        UserManager<User> userManager,
        JwtSettings jwtSettings,
        IGenericRepository<RefreshToken> refreshTokenRepo)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings;
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (request == null)
            throw new Core.Exceptions.ValidationException("Registration data is missing.");

        if (request.Password != request.ConfirmPassword)
            throw new Core.Exceptions.ValidationException("Passwords do not match.");

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Core.Exceptions.ValidationException("A user with this email already exists.");

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new Core.Exceptions.ValidationException(errors);
        }

        await _userManager.AddToRoleAsync(user, Roles.User);

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
            throw new UnauthorizedException("Invalid email or password.");

        var isValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isValid)
            throw new UnauthorizedException("Invalid email or password.");

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var storedToken = (await _refreshTokenRepo.GetAllAsync())
            .FirstOrDefault(rt => rt.Token == request.RefreshToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        // Revoke old token
        storedToken.IsRevoked = true;
        await _refreshTokenRepo.UpdateAsync(storedToken);

        var user = await _userManager.FindByIdAsync(storedToken.UserId);
        if (user == null || !user.IsActive)
            throw new UnauthorizedException("User not found or deactivated.");

        return await GenerateAuthResponse(user);
    }

    public async Task LogoutAsync(string userId, string refreshToken)
    {
        var storedToken = (await _refreshTokenRepo.GetAllAsync())
            .FirstOrDefault(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (storedToken != null)
        {
            storedToken.IsRevoked = true;
            await _refreshTokenRepo.UpdateAsync(storedToken);
        }
    }

    public async Task<UserInfoDto> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        var roles = await _userManager.GetRolesAsync(user);

        return new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = roles.FirstOrDefault() ?? Roles.User,
            LoyaltyPoints = user.LoyaltyPoints,
            HotelId = user.HotelId
        };
    }

    private async Task<AuthResponseDto> GenerateAuthResponse(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? Roles.User;

        var accessToken = JwtHelper.GenerateToken(user, role, _jwtSettings);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshExpiryDays),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepo.AddAsync(refreshTokenEntity);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            User = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role,
                LoyaltyPoints = user.LoyaltyPoints,
                HotelId = user.HotelId
            }
        };
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
