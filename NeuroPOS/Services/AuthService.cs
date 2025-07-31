using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Services
{
    

    public class AuthService
    {
        private const string KeyIsLoggedIn = "auth_is_logged_in";
        private const string KeyUserRole = "auth_user_role";
        private const string KeyExpiresAt = "auth_expires_at";

        public string? UserRole { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(UserRole);

        public bool Login(string username, string password, bool rememberMe = false)
        {
            string role = string.Empty;
           
            if (username == "admin" && password == "admin")
            {
                role = "Admin";
            }
            else if (username == "user" && password == "user")
            {
                role = "User";
            }
            else
            {
                return false;
            }

            UserRole = role;
            Preferences.Set(KeyIsLoggedIn, true);
            Preferences.Set(KeyUserRole, role);

            var expiresAt = rememberMe ? DateTime.Now.AddDays(7) : DateTime.Now.AddDays(1); 
            Preferences.Set(KeyExpiresAt, expiresAt.ToBinary()); 

            return true;
        }

        public void Logout()
        {
            UserRole = null;
            Preferences.Remove(KeyIsLoggedIn);
            Preferences.Remove(KeyUserRole);
            Preferences.Remove(KeyExpiresAt);
        }

        public bool IsAdmin => UserRole == "Admin";

        public bool IsTokenValid()
        {
            if (!Preferences.Get(KeyIsLoggedIn, false))
                return false;

            var expiresAt = DateTime.FromBinary(Preferences.Get(KeyExpiresAt, DateTime.Now.ToBinary()));
            if (DateTime.Now > expiresAt)
            {
                Logout();
                return false;
            }

            // Restore session if valid
            UserRole = Preferences.Get(KeyUserRole, (string)null);
            return true;
        }
    }
}
