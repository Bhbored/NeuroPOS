using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Services
{
    public class AuthService
    {
        public string? UserRole { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(UserRole);

        public bool Login(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                UserRole = "Admin";
                return true;
            }
            else if (username == "user" && password == "user")
            {
                UserRole = "User";
                return true;
            }
            return false;
        }

        public void Logout()
        {
            UserRole = null;
        }

        public bool IsAdmin => UserRole == "Admin";
    }
}
