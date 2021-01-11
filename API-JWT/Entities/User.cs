using System;
using System.Text.Json.Serialization;

namespace API_JWT.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public RefreshToken RefreshToken { get; set; }

        [JsonIgnore]
        public string Token { get; set; }
    }
}
