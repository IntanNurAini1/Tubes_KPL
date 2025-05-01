using System;
using Tubes_KPL.Model;

namespace Tubes_KPL.Controller
{
    public class UserController
    {
        private readonly UserManager userManager;

        public UserController(UserManager userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public void Register(string username, string password)
        {
            userManager.Register(username, password);
        }

        public void Login(string username, string password)
        {
            userManager.Login(username, password);
        }

        public void Logout()
        {
            userManager.Logout();
        }

        public void AddUser(string username, string password)
        {
            userManager.AddUser(username, password);
        }

        public IEnumerable<string> GetUsernames()
        {
            return userManager.GetUsernames();
        }

        public State GetCurrentState()
        {
            return userManager.CurrentState;
        }
    }
}