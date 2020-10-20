using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace Identity.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<Audience> _settings;

        public AuthController(IOptions<Audience> settings)
        {
            this._settings = settings;
        }

        [Produces("application/json")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SignIn([FromBody] UserLoginRequest request)
        {
            try
            {
                if (request.Name == "catcher" && request.Pwd == "123")
                {

                    var now = DateTime.UtcNow;

                    var claims = new Claim[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, request.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
                    };

                    var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Value.Secret));
                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateIssuer = true,
                        ValidIssuer = _settings.Value.Iss,
                        ValidateAudience = true,
                        ValidAudience = _settings.Value.Aud,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true,

                    };

                    var jwt = new JwtSecurityToken(
                        issuer: _settings.Value.Iss,
                        audience: _settings.Value.Aud,
                        claims: claims,
                        notBefore: now,
                        expires: now.Add(TimeSpan.FromMinutes(2)),
                        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                    );
                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                    var responseJson = new
                    {
                        access_token = encodedJwt,
                        expires_in = (int)TimeSpan.FromMinutes(2).TotalSeconds
                    };
                    return StatusCode(StatusCodes.Status201Created, responseJson);
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
               

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Sorry We are unable to register device");
            }
        }
    }

    public class Audience
    {
        public string Secret { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
    }

    public class UserLoginRequest
    {
        public string Name { get; set; }
        public string Pwd { get; set; }
    }

}

