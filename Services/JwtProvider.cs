using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using YTNotifier.Api.Entities;

namespace YTNotifier.Api.Services;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;

    public JwtProvider(IOptions<JwtOptions> jwtSettings)
    {
        _jwtOptions = jwtSettings.Value;
    }

    public async Task<(string Token, int ExpiresIn)> GenerateTokenAsync(ApplicationUser user)
    {
        var expiresIn = _jwtOptions.ExpiresIn;

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Aud, _jwtOptions.Audience),
            new(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddSeconds(expiresIn).ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        ];

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SecretKey));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(claims: claims, signingCredentials: signingCredentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresIn);
    }
}
