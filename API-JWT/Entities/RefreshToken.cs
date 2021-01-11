using System;
namespace API_JWT.Entities
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime ExpiredTime { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiredTime;

        public RefreshToken(string token, DateTime expiredTime)
        {
            ExpiredTime = expiredTime;
            Token = token;
        }
    }
}
