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
using BomRepo.BRMaster.DL;

namespace BomRepo.REST.API
{
    public class TokenProviderMiddleware
    {
        private const string useragentautocad = "autocad";
        private const string useragentinventor = "inventor";
        private const string useragentwebapi = "web.api";
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;

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

            //Verify request is comming from no authorized user agents
            if (userAgent != useragentautocad & userAgent != useragentinventor & userAgent != useragentwebapi) {
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

            //Define if token was issued by us cheking audience
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

            //Save it in db
            using (var db = new BRMasterModel()) {
                UsersManager usersMan = new UsersManager(db);
                bool updated = usersMan.SetToken(username, UserAgent, encodedJwt);
            }

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        private Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            try
            {
                using (var db = new BRMasterModel())
                {
                    UsersManager usersMan = new UsersManager(db);
                    User user = usersMan.Get(username, password);
                    if (user == null) return Task.FromResult<ClaimsIdentity>(null);

                    return Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(username, "Token"), new Claim[] { }));
                }
            }
            catch (Exception e) {
                return null;
            }
        }

        private bool TokenExists(string username, string useragent, string token) {
            using (var db = new BRMasterModel()) {
                UsersManager usersMan = new UsersManager(db);
                User user = usersMan.GetByToken(token);
                if (user == null) return false;

                if (useragent.ToLower() == useragentautocad) if (user.AutocadToken == token) return true;
                if (useragent.ToLower() == useragentinventor) if (user.InventorToken == token) return true;
                if (useragent.ToLower() == useragentwebapi) if (user.WebToken == token) return true;

                return false;
            }
        }
    }

    public class TokenProviderOptions {
        public string Path { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(2);
        public SigningCredentials SigningCredentials { get; set; }
    }
}
