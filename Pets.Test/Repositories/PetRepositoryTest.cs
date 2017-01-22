using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Pets.Model;
using Pets.Repositories;

namespace Pets.Test.Repositories
{
    [TestFixture]
    public class PetRepositoryTest
    {
        [Test]
        public void ShouldReturnAllPets()
        {
            var userName1 = "Alex";
            var userName2 = "Joe";
            var pet1 = new Pet { Name = "Toby", Type = PetType.Cat };
            var pet2 = new Pet { Name = "Buddy", Type = PetType.Dog };
            var pet3 = new Pet { Name = "Jack", Type = PetType.Parrot };

            var repository = new PetRepository();

            repository.UpdateUsersPet(userName1, pet1);
            repository.UpdateUsersPet(userName1, pet2);
            repository.UpdateUsersPet(userName2, pet3);

            var allPets = repository.GetAllPets().ToList();

            allPets.Should().Contain(pl => pl.Key == userName1);
            allPets.Should().Contain(pl => pl.Key == userName2);

            allPets.First(pl => pl.Key == userName1).Value.Count().Should().Be(2);
            allPets.First(pl => pl.Key == userName2).Value.Count().Should().Be(1);

            allPets.First(pl => pl.Key == userName1).Value.Should().Contain(pet1);
            allPets.First(pl => pl.Key == userName1).Value.Should().Contain(pet2);
            allPets.First(pl => pl.Key == userName2).Value.Should().Contain(pet3);
        }

        [Test]
        public void ShouldReturnEmptyUsersPetsListIfThereIsNoPetsAdded()
        {
            var repository = new PetRepository();
            repository.GetUsersPets("Alex").Should().BeEmpty();
        }

        [Test]
        public void ShouldReturnUsersPetsListIfPetsAdded()
        {
            var userName = "Alex";
            var pet = new Pet { Name = "Toby", Type = PetType.Cat };

            var repository = new PetRepository();
            repository.UpdateUsersPet(userName, pet);

            repository.GetUsersPets(userName).Should().Contain(pet);
        }

        [Test]
        public void ShouldReturnEmptyPetIfThereIsNoPetAdded()
        {
            var repository = new PetRepository();
            repository.GetUsersPet("Alex", "Toby").IsEmpty().Should().BeTrue();
        }

        [Test]
        public void ShouldReturnUsersPetIfPetAdded()
        {
            var userName = "Alex";
            var pet = new Pet { Name = "Toby", Type = PetType.Cat };

            var repository = new PetRepository();
            repository.UpdateUsersPet(userName, pet);

            repository.GetUsersPet(userName, pet.Name).Should().Be(pet);
        }

        [Test]
        public void ShouldUpdatePet()
        {
            var userName = "Alex";
            var pet = new Pet { Name = "Toby", Type = PetType.Cat };

            var repository = new PetRepository();

            repository.UpdateUsersPet(userName, pet);
            repository.GetUsersPet(userName, pet.Name).Should().Be(pet);

            // TODO : change pet
            pet.Name = "Buddy";
            pet.Type = PetType.Parrot;

            repository.UpdateUsersPet(userName, pet);
            repository.GetUsersPet(userName, pet.Name).Should().Be(pet);
        }

        [Test]
        public void ShouldNotRemoveUsersPetIfNotAdded()
        {
            var userName = "Alex";
            var petName = "Toby";

            var repository = new PetRepository();

            repository.RemoveUsersPet(userName, petName).Should().BeFalse();
        }

        [Test]
        public void ShouldNotRemoveUsersPetIfAlreadyRemoved()
        {
            var userName = "Alex";
            var pet = new Pet { Name = "Toby" };

            var repository = new PetRepository();
            repository.UpdateUsersPet(userName, pet);

            repository.RemoveUsersPet(userName, pet.Name).Should().BeTrue();
            repository.RemoveUsersPet(userName, pet.Name).Should().BeFalse();
        }

        [Test]
        public void ShouldRemoveUsersPet()
        {
            var userName = "Alex";
            var pet = new Pet { Name = "Toby" };

            var repository = new PetRepository();
            repository.UpdateUsersPet(userName, pet);

            repository.GetUsersPets(userName).Should().Contain(pet);
            repository.RemoveUsersPet(userName, pet.Name).Should().BeTrue();
            repository.GetUsersPets(userName).Should().NotContain(pet);
        }
    }
}