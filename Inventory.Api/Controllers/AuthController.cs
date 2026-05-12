using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var configuredUsername = _configuration["Auth:Username"] ?? throw new InvalidOperationException("Auth:Username is required.");
        var configuredPassword = _configuration["Auth:Password"] ?? throw new InvalidOperationException("Auth:Password is required.");

        if (!string.Equals(request.Username, configuredUsername, StringComparison.Ordinal) ||
            !string.Equals(request.Password, configuredPassword, StringComparison.Ordinal))
        {
            return Unauthorized("Invalid username or password.");
        }

        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required.");
        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is required.");
        var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is required.");

        var expiresMinutesValue = _configuration["Jwt:ExpiresMinutes"] ?? throw new InvalidOperationException("Jwt:ExpiresMinutes is required.");
        if (!int.TryParse(expiresMinutesValue, out var expiresMinutes) || expiresMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt:ExpiresMinutes must be a positive integer.");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, request.Username)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new LoginResponse(tokenValue, expiresAt));
    }

    public sealed record LoginRequest(string Username, string Password);

    public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);
}
