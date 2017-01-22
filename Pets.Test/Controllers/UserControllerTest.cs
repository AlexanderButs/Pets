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
    public class UserControllerTest
    {
        [Test]
        public async void ShouldReturnBadRequestIfUserNameIsInvalidWhenCreate()
        {
            var user = new User { Name = "A" };

            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.PutAsJsonAsync(string.Format($"/api/users/{user.Name}"), user);
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                // Execute test against the web API.
                response = await server.HttpClient.GetAsync("/api/users");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<IEnumerable<User>>(result).ToList();
                users.Should().NotContain(user);
            }
        }

        [Test]
        public async void ShouldReturnNotFoundIfNoUser()
        {
            using (var server = TestServer.Create<Startup>())
            {
                // Execute test against the web API.
                var response = await server.HttpClient.GetAsync("/api/users/Alex");
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Test]
        public async void ShouldCreateAndReturnAllUsers()
        {
            var user1 = new User { Name = "Alex" };
            var user2 = new User { Name = "Joe" };

            using (var server = TestServer.Create<Startup>())
            {
                await CreateUser(server, user1);
                await CreateUser(server, user2);

                // Execute test against the web API.
                var response = await server.HttpClient.GetAsync("/api/users");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<IEnumerable<User>>(result).ToList();
                users.Should().Contain(user1);
                users.Should().Contain(user2);
            }
        }

        [Test]
        public async void ShouldCreateAndReturnUser()
        {
            var user1 = new User { Name = "Alex" };
            var user2 = new User { Name = "Joe" };

            using (var server = TestServer.Create<Startup>())
            {
                await CreateUser(server, user1);
                await CreateUser(server, user2);

                // Execute test against the web API.
                var response = await server.HttpClient.GetAsync($"/api/users/{user1.Name}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(result);
                user.Should().Be(user1);
            }
        }

        [Test]
        public async void ShouldReturnConflictIfUserAlreadyExistWhenCreateUser()
        {
            var user = new User { Name = "Alex" };

            using (var server = TestServer.Create<Startup>())
            {
                await CreateUser(server, user);

                // Execute test against the web API.
                var response = await server.HttpClient.PutAsJsonAsync(string.Format($"/api/users/{user.Name}"), user);
                response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            }
        }

        [Test]
        public async void ShouldCreateAndDeleteUser()
        {
            var user1 = new User { Name = "Alex" };
            var user2 = new User { Name = "Joe" };

            using (var server = TestServer.Create<Startup>())
            {
                await CreateUser(server, user1);
                await CreateUser(server, user2);

                // Execute test against the web API.
                var response = await server.HttpClient.GetAsync($"/api/users/{user1.Name}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(result);
                user.Should().Be(user1);

                response = await server.HttpClient.DeleteAsync($"/api/users/{user1.Name}");
                response.EnsureSuccessStatusCode();

                response = await server.HttpClient.GetAsync($"/api/users/{user1.Name}");
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);

                response = await server.HttpClient.GetAsync("/api/users");
                response.EnsureSuccessStatusCode();
                result = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<IEnumerable<User>>(result).ToList();
                users.Should().NotContain(user1);
            }
        }

        public static async Task CreateUser(TestServer server, User user)
        {
            var response = await server.HttpClient.PutAsJsonAsync(string.Format($"/api/users/{user.Name}"), user);
            response.EnsureSuccessStatusCode();
        }
    }
}