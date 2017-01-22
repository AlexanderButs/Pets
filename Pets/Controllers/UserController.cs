using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Pets.Model;
using Pets.Repositories;
using Swashbuckle.Swagger.Annotations;

namespace Pets.Controllers
{
    [RoutePrefix("api/users")]
    public class UserController : ApiController
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Use this method to get all registered users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<User>))]
        public IEnumerable<User> GetAll()
        {
            return _userRepository.GetAllUsers();
        }

        /// <summary>
        /// Use this method to get user by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{name}")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(User))]
        public IHttpActionResult GetByName(string name)
        {
            var user = _userRepository.GetUser(name);
            if (user.IsEmpty())
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Use this method to register new user.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{name}")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.Conflict)]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(User))]
        public IHttpActionResult Create(string name, [FromBody] User user)
        {
            if (!ModelState.IsValid || user.Name != name)
            {
                return BadRequest();
            }

            // NOTE: multithreaded issue
            var exist = _userRepository.GetAllUsers().Any(u => u.Name == name);
            if (exist)
            {
                return Conflict();
            }

            _userRepository.UpdateUser(user);

            Console.WriteLine($"User '{user}' has been created.");

            return Created(string.Format($"api/users/{name}"), user);
        }

        /// <summary>
        /// Use this method to delete registered user.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{name}")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult DeleteByName(string name)
        {
            _userRepository.RemoveUser(name);

            Console.WriteLine($"User '{name}' has been removed.");

            return Ok();
        }
    }
}