using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API_JWT.Entities;
using API_JWT.Helpers;
using API_JWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API_JWT.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
        string generateToken(User user);
        AuthenticateResponse RenewToken(string token, string refreshToken);
    }

    public class UserService : IUserService
    {
        private List<User> _users = null;

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings, List<User> users)
        {
            _appSettings = appSettings.Value;
            _users = users;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            // return null if user not found
            if (user == null) throw new Exception("Username or password wrong !");

            // authentication successful so generate jwt token
            var token = this.generateToken(user);

            var refreshToken = generateRefreshToken();
            _users.ForEach(x =>
            {
                if (x.Id == user.Id)
                {
                    x.RefreshToken = refreshToken;
                    x.Token = token;
                }
            });
            return new AuthenticateResponse(user, token,refreshToken.Token);
        }

        public AuthenticateResponse RenewToken(string token, string refreshToken)
       {
            var user = _users.Where(x => refreshToken == x.RefreshToken?.Token && token.Equals(x.Token)).FirstOrDefault();

            if (user == null || user.RefreshToken.IsExpired) throw new Exception("Invalid Refresh Token");

            var newRefreshToken = generateRefreshToken();
            var newToken = generateToken(user);
            _users.ForEach(x =>
            {
                if (x.Id == user.Id)
                {
                    x.RefreshToken = newRefreshToken;
                    x.Token = newToken;
                }
            });
            return new AuthenticateResponse(user, newToken, newRefreshToken.Token);
        }

        private RefreshToken generateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var token = Convert.ToBase64String(randomNumber);
                return new RefreshToken(token, DateTime.UtcNow.AddDays(_appSettings.RefreshTokenExpiration));
            }
        }

        [Authorize]
        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        [Authorize]
        public User GetById(int id)
        {
            return _users.FirstOrDefault(x => x.Id == id);
        }


        public string generateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id",user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                _appSettings.Issuer,
                _appSettings.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_appSettings.AccessTokenExpiration),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
