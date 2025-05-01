using System;

namespace Tubes_KPL.Model
{
    public class User
    {
        public string Username { get; }
        private readonly string password;

        public User(string username, string password)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            this.password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public bool VerifyPassword(string inputPassword)
        {
            return password == inputPassword;
        }
    }
}