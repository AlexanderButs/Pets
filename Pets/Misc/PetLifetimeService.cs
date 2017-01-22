using System;
using System.Linq;
using System.Threading;
using Pets.Repositories;

namespace Pets.Misc
{
    public class PetLifetimeService
    {
        private readonly IPetRepository _petRepository;
        private readonly IPetAttributeContainer _petAttributeContainer;

        private readonly Timer _timer;

        public PetLifetimeService(IPetRepository petRepository, IPetAttributeContainer petAttributeContainer)
        {
            _petRepository = petRepository;
            _petAttributeContainer = petAttributeContainer;

            var updatePeriod = TimeSpan.FromMinutes(1);
            _timer = new Timer(OnTick, updatePeriod, updatePeriod, updatePeriod);
        }

        private void OnTick(object state)
        {
            var updatePeriod = (TimeSpan)state;

            var allPets = _petRepository.GetAllPets().ToList();
            foreach (var kv in allPets)
            {
                var userName = kv.Key;

                foreach (var pet in kv.Value.ToList())
                {
                    pet.DoNotFeedingFor(updatePeriod, _petAttributeContainer);
                    pet.DoNotPettingFor(updatePeriod, _petAttributeContainer);

                    _petRepository.UpdateUsersPet(userName, pet);

                    Console.WriteLine($"User's '{userName}' pet '{pet}' has lived for '{updatePeriod}'.");
                }
            }
        }
    }
}