using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }
        public class  CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }
        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            //In a real world application, you would use a token-based authentication system, such as JWT (JSON Web Tokens), to authenticate users.
            //In a real world application, you would also validate the user's credentials against a database or an external authentication provider.

            //Step 1: Validate the input
            var user = ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);
            
            if (user == null) {
                return Unauthorized();
            }
            //Step 2: Create the token
            var secureKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Authentication:SecretForkey"]));

            var credentials = new SigningCredentials(secureKey, SecurityAlgorithms.HmacSha256);

            //the claims we want to include in the token
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            var token = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            //Step 3: Return the token
            return Ok(tokenString);

        }
        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            //In a real world application, you would validate the user's credentials against a database or an external authentication provider.
            //For this example, we will just check if the username and password are not null or empty
            
            return new CityInfoUser ( 1, userName ?? "","Kevin","Sins","Antwerp");
        }
    }
}
