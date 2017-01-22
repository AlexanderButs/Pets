using System;
using System.ComponentModel.DataAnnotations;
using Pets.Misc;

namespace Pets.Model
{
    public enum PetType
    {
        Dog = 0,
        Cat,
        Parrot
    }

    public struct Pet : IEquatable<Pet>
    {
        public static int DefaultHappiness = 50;
        public static int DefaultHunger = 50;

        public static Pet Empty = new Pet { Name = string.Empty };

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        public PetType Type { get; set; }

        public int Happiness { get; set; }

        public int Hunger { get; set; }

        public bool IsEmpty()
        {
            return Equals(Empty);
        }

        public bool Equals(Pet other)
        {
            return Name == other.Name && Type == other.Type && Happiness == other.Happiness && Hunger == other.Hunger;
        }

        public void DoPettingFor(TimeSpan duration, IPetAttributeContainer attributeContainer)
        {
            if (duration <= TimeSpan.Zero)
            {
                throw new ArgumentException("duration");
            }

            var happiness = attributeContainer.GetHappinessIncreaseByPetting(Type, duration);
            Happiness = Math.Min(100, Happiness + happiness);
        }

        public void DoNotPettingFor(TimeSpan duration, IPetAttributeContainer attributeContainer)
        {
            if (duration <= TimeSpan.Zero)
            {
                throw new ArgumentException("duration");
            }

            var happiness = attributeContainer.GetHappinessIncreaseByPetting(Type, duration);
            Happiness = Math.Max(0, Happiness - happiness);
        }

        public void DoFeedingWith(uint food, IPetAttributeContainer attributeContainer)
        {
            if (food == 0)
            {
                throw new ArgumentException("food");
            }

            var hunger = attributeContainer.GetHungerDecreaseByFood(Type, food);
            Hunger = Math.Max(0, Hunger - hunger);
        }

        public void DoNotFeedingFor(TimeSpan duration, IPetAttributeContainer attributeContainer)
        {
            if (duration <= TimeSpan.Zero)
            {
                throw new ArgumentException("duration");
            }

            var hunger = attributeContainer.GetHungerIncreaseByTime(Type, duration);
            Hunger = Math.Min(100, Hunger + hunger);
        }

        public override string ToString()
        {
            return string.Format($"<Pet Name='{Name}' Type='{Type}' Happiness='{Happiness}' Hunger='{Hunger}'>");
        }
    }
}