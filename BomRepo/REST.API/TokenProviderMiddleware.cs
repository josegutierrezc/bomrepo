using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using BomRepo.BRMaster.DTO;

namespace BomRepo.REST.API
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;

        //just for testing proposes
        private List<UserDTO> usersRepo = new List<UserDTO>() {
            new UserDTO() { Id = 1, Username = "jose", Password = "e10adc3949ba59abbe56e057f20f883e" },
            new UserDTO() { Id = 2, Username = "jesus", Password = "e10adc3949ba59abbe56e057f20f883e" }
        };
        private List<UserAccessTokensDTO> userAccessTokensRepo = new List<UserAccessTokensDTO>() {
            new UserAccessTokensDTO() { UserId = 1, App = "autocad", AccessToken = "" },
            new UserAccessTokensDTO() { UserId = 2, App = "autocad", AccessToken = "" }
        };

        public TokenProviderMiddleware(RequestDelegate next, IOptions<TokenProviderOptions> options) {
            _next = next;
            _options = options.Value;
        }

        public Task Invoke(HttpContext context)
        {
            //All request have to contain Authorization and User-Agent headers 
            if (!context.Request.Headers.ContainsKey("Authorization") || !context.Request.Headers.ContainsKey("User-Agent"))
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            //Get Authorization and User-Agent Header values
            Tuple<string, string> authHeader = Helper.GetAuthorizationHeaderValuesFromString(context.Request.Headers["Authorization"].ToString());
            string userAgent = context.Request.Headers["User-Agent"].ToString().ToLower();

            // If the request path doesn't match, then validate token
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                //If it is a valid token then move request to the controller
                if (IsValidToken(context, authHeader, userAgent)) return _next(context);
                
                //If it is not a valid token then return error
                context.Response.StatusCode = 401;
                return context.Response.WriteAsync("Unauthorized.");
            }

            // Request must be POST
            if (!context.Request.Method.Equals("POST")) {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            //Verify request is comming from authorized user agents
            if (userAgent != "autocad" & userAgent != "inventor" & userAgent != "web.api") {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            //Return the new token
            return GenerateToken(context, authHeader, userAgent);
        }

        private bool IsValidToken(HttpContext context, Tuple<string, string> AuthHeaderValues, string UserAgentHeaderValue) {
            if (!AuthHeaderValues.Item1.ToLower().StartsWith("bearer")) return false;

            //Get jwt
            var decodedJwt = new JwtSecurityTokenHandler().ReadJwtToken(AuthHeaderValues.Item2);

            //Define if token was issued by us
            if (decodedJwt.Issuer != _options.Issuer | decodedJwt.Audiences.FirstOrDefault() != _options.Audience) return false;

            //Define if token has expired
            if (decodedJwt.ValidTo < DateTime.UtcNow) return false;

            //Get username from claim
            string username = decodedJwt.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault().Value;

            //VErify token exist and is assigned to the right user
            return TokenExists(username, UserAgentHeaderValue, AuthHeaderValues.Item2);
        }

        private async Task GenerateToken(HttpContext context, Tuple<string, string> AuthHeaderValues, string UserAgent)
        {
            string username = string.Empty;
            string password = string.Empty;
            if (AuthHeaderValues.Item1.ToLower().StartsWith("basic"))
            {
                Tuple<string, string> usernamepassword = Helper.GetUsernameAndPasswordFromEncodedString(AuthHeaderValues.Item2);
                username = usernamepassword.Item1;
                password = usernamepassword.Item2;
            }

            var identity = await GetIdentity(username, password);
            if (identity == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            var now = DateTime.UtcNow;

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        private Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var user = usersRepo.Where(u => u.Username == username & u.Password == password).FirstOrDefault();
            if (user == null) return Task.FromResult<ClaimsIdentity>(null);

            return Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(username, "Token"), new Claim[] { }));
        }

        private bool TokenExists(string Username, string App, string Token) {
            var result = from u in usersRepo
                         join t in userAccessTokensRepo on u.Id equals t.UserId
                         where u.Username == Username & t.App == App
                         select t;
            return result.FirstOrDefault() != null;
        }
    }

    public class TokenProviderOptions {
        public string Path { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(1);
        public SigningCredentials SigningCredentials { get; set; }
    }
}
