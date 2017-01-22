using System.Collections.Generic;
using System.Linq;
using Pets.Model;

namespace Pets.Repositories
{
    public interface IPetRepository
    {
        IEnumerable<KeyValuePair<string, IEnumerable<Pet>>> GetAllPets();

        IEnumerable<Pet> GetUsersPets(string userName);

        Pet GetUsersPet(string userName, string petName);

        void UpdateUsersPet(string userName, Pet pet);

        bool RemoveUsersPet(string userName, string petName);
    }

    public class PetRepository : IPetRepository
    {
        private readonly object _lock = new object();

        private readonly Dictionary<string, List<Pet>> _usersPets = new Dictionary<string, List<Pet>>();

        public IEnumerable<KeyValuePair<string, IEnumerable<Pet>>> GetAllPets()
        {
            lock (_lock)
            {
                foreach (var kv in _usersPets)
                {
                    yield return new KeyValuePair<string, IEnumerable<Pet>>(kv.Key, kv.Value);
                }
            }
        }

        public IEnumerable<Pet> GetUsersPets(string userName)
        {
            lock (_lock)
            {
                List<Pet> usersPets;
                if (_usersPets.TryGetValue(userName, out usersPets))
                {
                    return usersPets.ToList();
                }
            }

            return Enumerable.Empty<Pet>();
        }

        public Pet GetUsersPet(string userName, string petName)
        {
            lock (_lock)
            {
                List<Pet> usersPets;
                if (_usersPets.TryGetValue(userName, out usersPets))
                {
                    var pet = usersPets.FirstOrDefault(p => p.Name == petName);
                    if (!pet.Equals(default(Pet)))
                    {
                        return pet;
                    }

                    return Pet.Empty;
                }

                return Pet.Empty;
            }
        }

        public void UpdateUsersPet(string userName, Pet pet)
        {
            lock (_lock)
            {
                List<Pet> usersPets;
                if (!_usersPets.TryGetValue(userName, out usersPets))
                {
                    _usersPets.Add(userName, new List<Pet> { pet });
                    return;
                }

                var index = usersPets.FindIndex(p => p.Name == pet.Name);
                if (index < 0)
                {
                    usersPets.Add(pet);
                    return;
                }

                usersPets[index] = pet;
            }
        }

        public bool RemoveUsersPet(string userName, string petName)
        {
            lock (_lock)
            {
                List<Pet> usersPets;
                if (!_usersPets.TryGetValue(userName, out usersPets))
                {
                    return false;
                }

                return usersPets.RemoveAll(p => p.Name == petName) > 0;
            }
        }
    }
}