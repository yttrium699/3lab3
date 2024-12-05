using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("user")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly IRepository<User> usersRepository;

        public UsersController(ILogger<UsersController> logger, IRepository<User> usersRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            logger.LogInformation("Get all users");
            return Ok(await usersRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(Guid id)
        {
            var user = await usersRepository.GetByIdAsync(id);
            if (user == null) return NotFound($"Пользователь с ID {id} не найден.");

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] User user)
        {
            var validationResult = ValidateUser(user);
            if (!string.IsNullOrWhiteSpace(validationResult))
                return BadRequest(validationResult);

            if (!user.Id.HasValue)
                user.Id = Guid.NewGuid();

            return Ok(await usersRepository.InsertAsync(user));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] User user)
        {
            var validationResult = ValidateUser(user);
            if (!string.IsNullOrWhiteSpace(validationResult))
                return BadRequest(validationResult);

            if (await usersRepository.GetByIdAsync(id) == null)
                return NotFound($"Пользователь с ID {id} не существует.");

            await usersRepository.UpdateAsync(id, user);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            if (await usersRepository.GetByIdAsync(id) == null)
                return NotFound($"Пользователь с ID {id} не существует.");

            await usersRepository.DeleteAsync(id);
            return Ok();
        }

        private static string ValidateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Login))
                return "Логин не может быть пустым.";
            if (string.IsNullOrWhiteSpace(user.PassHash))
                return "Пароль не может быть пустым.";

            return string.Empty;
        }
    }
}
