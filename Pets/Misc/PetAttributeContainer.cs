using System;
using System.Collections.Generic;
using Pets.Model;

namespace Pets.Misc
{
    public interface IPetAttributeContainer
    {
        int GetHappinessIncreaseByPetting(PetType petType, TimeSpan duration);

        int GetHungerDecreaseByFood(PetType petType, uint food);

        int GetHungerIncreaseByTime(PetType petType, TimeSpan duration);
    }

    public class PetAttributeContainer : IPetAttributeContainer
    {
        // happiness increase/decrease per minute petting/not petting
        private readonly Dictionary<PetType, int> _petsHappinessSpeed = new Dictionary<PetType, int>
        {
            { PetType.Dog, 10 },
            { PetType.Cat, 3 },
            { PetType.Parrot, 1 },
        };

        // hunger increase per minute not feeding
        private readonly Dictionary<PetType, int> _petsHungerSpeed = new Dictionary<PetType, int>
        {
            { PetType.Dog, 25 },
            { PetType.Cat, 2 },
            { PetType.Parrot, 3 },
        };

        public int GetHappinessIncreaseByPetting(PetType petType, TimeSpan duration)
        {
            return _petsHappinessSpeed[petType] * (int)duration.TotalMinutes;
        }

        public int GetHungerDecreaseByFood(PetType petType, uint food)
        {
            // one food item is equal to one minute not feeding
            return _petsHungerSpeed[petType] * (int)food;
        }

        public int GetHungerIncreaseByTime(PetType petType, TimeSpan duration)
        {
            return _petsHungerSpeed[petType] * (int)duration.TotalMinutes;
        }
    }
}