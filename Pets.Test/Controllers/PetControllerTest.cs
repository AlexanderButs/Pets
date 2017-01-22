using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Owin.Testing;

using Newtonsoft.Json;

using NUnit.Framework;
using Pets.Model;

namespace Pets.Test.Controllers
{
    [TestFixture]
    public class PetControllerTest
    {
        [Test]
        public async void ShouldReturnBadRequestIfPetNameIsInvalidWhenCreate()
        {
            var user = new User { Name = "Alex" };
            var pet = new Pet { Name = "A" };

            using (var server = TestServer.Create<Startup>())
            {
                await UserControllerTest.CreateUser(server, user);

                var response = await server.HttpClient.PutAsJsonAsync(string.Format($"/api/users/{user.Name}/pets/{pet.Name}"), pet);
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                // Execute test against the web API.
                response = await server.HttpClient.GetAsync(string.Format($"/api/users/{user.Name}/pets"));
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var pets = JsonConvert.DeserializeObject<IEnumerable<Pet>>(result).ToList();
                pets.Should().NotContain(pet);
            }
        }

        [Test]
        public async void ShouldCreateAndReturnAllUserPets()
        {
            var user1 = new User { Name = "Alex" };
            var user2 = new User { Name = "Joe" };

            var pet1 = new Pet { Name = "Buddy", Happiness = Pet.DefaultHappiness, Hunger = Pet.DefaultHunger };
            var pet2 = new Pet { Name = "Jack", Happiness = Pet.DefaultHappiness, Hunger = Pet.DefaultHunger };
            var pet3 = new Pet { Name = "Toby", Happiness = Pet.DefaultHappiness, Hunger = Pet.DefaultHunger };

            using (var server = TestServer.Create<Startup>())
            {
                await UserControllerTest.CreateUser(server, user1);
                await UserControllerTest.CreateUser(server, user2);

                await CreateUsersPet(server, user1.Name, pet1);
                await CreateUsersPet(server, user1.Name, pet2);
                await CreateUsersPet(server, user2.Name, pet3);

                // Execute test against the web API.
                var response = await server.HttpClient.GetAsync($"/api/users/{user1.Name}/pets");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var pets = JsonConvert.DeserializeObject<IEnumerable<Pet>>(result).ToList();
                pets.Should().Contain(pet1);
                pets.Should().Contain(pet2);
                pets.Should().NotContain(pet3);
            }
        }

        [Test]
        public async void ShouldReturnNotFoundIfNoPet()
        {
            using (var server = TestServer.Create<Startup>())
            {
                // Execute test against the web API.
                var response = await server.HttpClient.GetAsync("/api/users/Alex/pets/Toby");
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Test]
        public async void ShouldReturnNotFoundIfNoUserWhenGetUsersPets()
        {
            using (var server = TestServer.Create<Startup>())
            {
                // Execute test against the web API.
                var response = await server.HttpClient.GetAsync("/api/users/Alex/pets");
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Test]
        public async void ShouldReturnNotFoundIfNoUserWhenGetUsersPet()
        {
            using (var server = TestServer.Create<Startup>())
            {
                // Execute test against the web API.
                var response = await server.HttpClient.GetAsync("/api/users/Alex/pets/Toby");
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Test]
        public async void ShouldCreateAndReturnUsersPet()
        {
            var user1 = new User { Name = "Alex" };
            var user2 = new User { Name = "Joe" };

            var pet1 = new Pet { Name = "Buddy", Type = PetType.Cat, Happiness = Pet.DefaultHappiness, Hunger = Pet.DefaultHunger };
            var pet2 = new Pet { Name = "Jack", Type = PetType.Dog, Happiness = Pet.DefaultHappiness, Hunger = Pet.DefaultHunger };
            var pet3 = new Pet { Name = "Toby", Type = PetType.Parrot, Happiness = Pet.DefaultHappiness, Hunger = Pet.DefaultHunger };

            using (var server = TestServer.Create<Startup>())
            {
                await UserControllerTest.CreateUser(server, user1);
                await UserControllerTest.CreateUser(server, user2);

                await CreateUsersPet(server, user1.Name, pet1);
                await CreateUsersPet(server, user1.Name, pet2);
                await CreateUsersPet(server, user2.Name, pet3);

                // Execute test against the web API.
                var response = await server.HttpClient.GetAsync($"/api/users/{user1.Name}/pets/{pet1.Name}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var pet = JsonConvert.DeserializeObject<Pet>(result);
                pet.Should().Be(pet1);
            }
        }

        [Test]
        public async void ShouldReturnNotFoundIfNoUserWhenCreateUsersPet()
        {
            var pet = new Pet { Name = "Buddy" };

            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.PutAsJsonAsync(string.Format($"/api/users/Alex/pets/{pet.Name}"), pet);
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Test]
        public async void ShouldReturnNotFoundIfNoUserWhenPettingUsersPet()
        {
            var duration = TimeSpan.FromMinutes(5);

            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.PostAsJsonAsync($"/api/users/Alex/pets/Buddy/petting/{duration}", string.Empty);
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Test]
        public async void ShouldReturnNotFoundIfNoPetWhenPettingUsersPet()
        {
            var duration = TimeSpan.FromMinutes(5);
            var user = new User { Name = "Alex" };

            using (var server = TestServer.Create<Startup>())
            {
                await UserControllerTest.CreateUser(server, user);

                var response = await server.HttpClient.PostAsJsonAsync($"/api/users/{user.Name}/pets/Buddy/petting/{duration}", string.Empty);
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Test]
        public async void ShouldReturnOkWhenPetUsersPet()
        {
            var duration = TimeSpan.FromMinutes(5);
            var user = new User { Name = "Alex" };
            var pet = new Pet { Name = "Buddy" };

            using (var server = TestServer.Create<Startup>())
            {
                await UserControllerTest.CreateUser(server, user);
                await CreateUsersPet(server, user.Name, pet);

                var response = await server.HttpClient.PostAsJsonAsync($"/api/users/{user.Name}/pets/{pet.Name}/petting/{duration}", string.Empty);
                response.EnsureSuccessStatusCode();
            }
        }

        [Test]
        public async void ShouldIncreaseHappinessWhenPetUsersPet()
        {
            var duration = TimeSpan.FromMinutes(5);
            var user = new User { Name = "Alex" };
            var pet = new Pet { Name = "Buddy" };

            using (var server = TestServer.Create<Startup>())
            {
                await UserControllerTest.CreateUser(server, user);
                await CreateUsersPet(server, user.Name, pet);

                // get pet after creation
                var response = await server.HttpClient.GetAsync($"/api/users/{user.Name}/pets/{pet.Name}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var createdPet = JsonConvert.DeserializeObject<Pet>(result);

                // petting pet
                response = await server.HttpClient.PostAsJsonAsync($"/api/users/{user.Name}/pets/{pet.Name}/petting/{duration}", string.Empty);
                response.EnsureSuccessStatusCode();

                // get pet after petting
                response = await server.HttpClient.GetAsync($"/api/users/{user.Name}/pets/{pet.Name}");
                response.EnsureSuccessStatusCode();
                result = await response.Content.ReadAsStringAsync();
                var newPet = JsonConvert.DeserializeObject<Pet>(result);

                // check happiness is increased
                createdPet.Happiness.Should().BeLessThan(newPet.Happiness);
            }
        }

        [Test]
        public async void ShouldReturnNotFoundIfNoUserWhenFeedingUsersPet()
        {
            var food = 5;

            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.PostAsJsonAsync($"/api/users/Alex/pets/Buddy/feeding/{food}", string.Empty);
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Test]
        public async void ShouldReturnNotFoundIfNoPetWhenFeedingUsersPet()
        {
            var food = 5;
            var user = new User { Name = "Alex" };

            using (var server = TestServer.Create<Startup>())
            {
                await UserControllerTest.CreateUser(server, user);

                var response = await server.HttpClient.PostAsJsonAsync($"/api/users/{user.Name}/pets/Buddy/feeding/{food}", string.Empty);
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Test]
        public async void ShouldDecreaseHungerWhenFeedingUsersPet()
        {
            var food = 5;
            var user = new User { Name = "Alex" };
            var pet = new Pet { Name = "Buddy" };

            using (var server = TestServer.Create<Startup>())
            {
                await UserControllerTest.CreateUser(server, user);
                await CreateUsersPet(server, user.Name, pet);

                // get pet after creation
                var response = await server.HttpClient.GetAsync($"/api/users/{user.Name}/pets/{pet.Name}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var createdPet = JsonConvert.DeserializeObject<Pet>(result);

                // petting pet
                response = await server.HttpClient.PostAsJsonAsync($"/api/users/{user.Name}/pets/{pet.Name}/feeding/{food}", string.Empty);
                response.EnsureSuccessStatusCode();

                // get pet after petting
                response = await server.HttpClient.GetAsync($"/api/users/{user.Name}/pets/{pet.Name}");
                response.EnsureSuccessStatusCode();
                result = await response.Content.ReadAsStringAsync();
                var newPet = JsonConvert.DeserializeObject<Pet>(result);

                // check happiness is increased
                createdPet.Hunger.Should().BeGreaterThan(newPet.Hunger);
            }
        }

        [Test]
        public async void ShouldReturnOkWhenFeedUsersPet()
        {
            var food = 5;
            var user = new User { Name = "Alex" };
            var pet = new Pet { Name = "Buddy" };

            using (var server = TestServer.Create<Startup>())
            {
                await UserControllerTest.CreateUser(server, user);
                await CreateUsersPet(server, user.Name, pet);

                var response = await server.HttpClient.PostAsJsonAsync($"/api/users/{user.Name}/pets/Buddy/feeding/{food}", string.Empty);
                response.EnsureSuccessStatusCode();
            }
        }

        [Test]
        public async void ShouldCreateAndDeleteUsersPet()
        {
            var user = new User { Name = "Alex" };

            var pet1 = new Pet { Name = "Buddy", Happiness = Pet.DefaultHappiness, Hunger = Pet.DefaultHunger };
            var pet2 = new Pet { Name = "Jack", Happiness = Pet.DefaultHappiness, Hunger = Pet.DefaultHunger };

            using (var server = TestServer.Create<Startup>())
            {
                await UserControllerTest.CreateUser(server, user);

                await CreateUsersPet(server, user.Name, pet1);
                await CreateUsersPet(server, user.Name, pet2);

                // Execute test against the web API.
                var response = await server.HttpClient.DeleteAsync($"/api/users/{user.Name}/pets/{pet1.Name}");
                response.EnsureSuccessStatusCode();

                response = await server.HttpClient.GetAsync($"/api/users/{user.Name}/pets");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var pets = JsonConvert.DeserializeObject<IEnumerable<Pet>>(result).ToList();
                pets.Should().NotContain(pet1);
                pets.Should().Contain(pet2);
            }
        }

        private async Task CreateUsersPet(TestServer server, string userName, Pet pet)
        {
            var response = await server.HttpClient.PutAsJsonAsync(string.Format($"/api/users/{userName}/pets/{pet.Name}"), pet);
            response.EnsureSuccessStatusCode();
        }
    }
}