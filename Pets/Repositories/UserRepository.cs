using System.Collections.Concurrent;
using System.Collections.Generic;
using Pets.Model;

namespace Pets.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();

        User GetUser(string name);

        void UpdateUser(User user);

        bool RemoveUser(string name);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();

        public IEnumerable<User> GetAllUsers()
        {
            return _users.Values;
        }

        public User GetUser(string name)
        {
            User user;
            if (_users.TryGetValue(name, out user))
            {
                return user;
            }

            return User.Empty;
        }

        public void UpdateUser(User user)
        {
            _users.AddOrUpdate(user.Name, user, (s, u) => user);
        }

        public bool RemoveUser(string name)
        {
            User user;
            return _users.TryRemove(name, out user);
        }
    }
}