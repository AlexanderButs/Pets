using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Pets.Misc;
using Pets.Model;
using Pets.Repositories;
using Swashbuckle.Swagger.Annotations;

namespace Pets.Controllers
{
    [RoutePrefix("api")]
    public class PetController : ApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IPetRepository _petRepository;
        private readonly IPetAttributeContainer _petAttributeContainer;

        public PetController(IUserRepository userRepository, IPetRepository petRepository, IPetAttributeContainer petAttributeContainer)
        {
            _userRepository = userRepository;
            _petRepository = petRepository;
            _petAttributeContainer = petAttributeContainer;
        }

        /// <summary>
        /// Use this method to get all users pets.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("users/{user_name}/pets")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<Pet>))]
        public IHttpActionResult GetAllUsersPets([FromUri(Name = "user_name")] string userName)
        {
            var user = _userRepository.GetUser(userName);
            if (user.IsEmpty())
            {
                return NotFound();
            }

            var usersPets = _petRepository.GetUsersPets(userName);
            return Ok(usersPets);
        }

        /// <summary>
        /// Use this method to get users pet by pet name.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="petName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("users/{user_name}/pets/{pet_name}")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Pet))]
        public IHttpActionResult GetUsersPetByName([FromUri(Name = "user_name")] string userName, [FromUri(Name = "pet_name")] string petName)
        {
            var user = _userRepository.GetUser(userName);
            if (user.IsEmpty())
            {
                return NotFound();
            }

            var pet = _petRepository.GetUsersPet(userName, petName);
            if (pet.IsEmpty())
            {
                return NotFound();
            }

            return Ok(pet);
        }

        /// <summary>
        /// Use this pet to create new pet for user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="petName"></param>
        /// <param name="pet"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("users/{user_name}/pets/{pet_name}")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Conflict)]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(Pet))]
        public IHttpActionResult CreateUsersPet([FromUri(Name = "user_name")] string userName, [FromUri(Name = "pet_name")] string petName, [FromBody] Pet pet)
        {
            if (!ModelState.IsValid || pet.Name != petName)
            {
                return BadRequest();
            }

            var user = _userRepository.GetUser(userName);
            if (user.IsEmpty())
            {
                return NotFound();
            }

            var usersPets = _petRepository.GetUsersPets(userName);
            if (usersPets.Any(p => p.Name == petName))
            {
                return Conflict();
            }

            pet.Happiness = Pet.DefaultHappiness;
            pet.Hunger = Pet.DefaultHunger;

            _petRepository.UpdateUsersPet(userName, pet);

            Console.WriteLine($"Pet '{pet}' has been created for user '{userName}'");

            return Created(string.Format($"api/users/{userName}/pets/{pet.Name}"), pet);
        }

        /// <summary>
        /// Use this method to pet users pet for some amount of time.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="petName"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("users/{user_name}/pets/{pet_name}/petting/{duration}")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Pet))]
        public IHttpActionResult PetUsersPet([FromUri(Name = "user_name")] string userName, [FromUri(Name = "pet_name")] string petName, [FromUri(Name = "duration")] TimeSpan duration)
        {
            var user = _userRepository.GetUser(userName);
            if (user.IsEmpty())
            {
                return NotFound();
            }

            var pet = _petRepository.GetUsersPet(userName, petName);
            if (pet.IsEmpty())
            {
                return NotFound();
            }

            pet.DoPettingFor(duration, _petAttributeContainer);
            _petRepository.UpdateUsersPet(user.Name, pet);

            Console.WriteLine($"Pet '{pet}' has been petted for '{duration}' by user '{userName}'");

            return Ok(pet);
        }

        /// <summary>
        /// Use this method to feed users pet with some amount of food.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="petName"></param>
        /// <param name="food"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("users/{user_name}/pets/{pet_name}/feeding/{food:min(1):max(100)}")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Pet))]
        public IHttpActionResult FeedUsersPet([FromUri(Name = "user_name")] string userName, [FromUri(Name = "pet_name")] string petName, [FromUri(Name = "food")] uint food)
        {
            var user = _userRepository.GetUser(userName);
            if (user.IsEmpty())
            {
                return NotFound();
            }

            var pet = _petRepository.GetUsersPet(userName, petName);
            if (pet.IsEmpty())
            {
                return NotFound();
            }

            pet.DoFeedingWith(food, _petAttributeContainer);
            _petRepository.UpdateUsersPet(user.Name, pet);

            Console.WriteLine($"Pet '{pet}' has been fed with '{food}' by user '{userName}'");

            return Ok(pet);
        }

        /// <summary>
        /// Use this method to delete users pet.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="petName"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("users/{user_name}/pets/{pet_name}")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult DeleteUsersPetByName([FromUri(Name = "user_name")] string userName, [FromUri(Name = "pet_name")] string petName)
        {
            _petRepository.RemoveUsersPet(userName, petName);

            Console.WriteLine($"Pet '{petName}' has been deleted for user '{userName}'");

            return Ok();
        }
    }
}