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
    [Route("emotion")]
    public class EmotionsController : ControllerBase
    {
        private readonly ILogger<EmotionsController> logger;
        private readonly IRepository<Emotion> emotionRepository;

        public EmotionsController(ILogger<EmotionsController> logger, IRepository<Emotion> emotionRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.emotionRepository = emotionRepository ?? throw new ArgumentNullException(nameof(emotionRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Emotion>>> Get()
        {
            logger.LogInformation("Get all emotions");
            return Ok(await emotionRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Emotion>> Get(Guid id)
        {
            var emotion = await emotionRepository.GetByIdAsync(id);
            if (emotion == null) return NotFound($"Эмоция с ID {id} не найдена.");

            return Ok(emotion);
        }

        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] Emotion emotion)
        {
            if (emotion == null) return BadRequest("Эмоция не может быть пустой.");

            if (!emotion.Id.HasValue)
                emotion.Id = Guid.NewGuid();

            return Ok(await emotionRepository.InsertAsync(emotion));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] Emotion emotion)
        {
            if (emotion == null) return BadRequest("Эмоция не может быть пустой.");

            if (await emotionRepository.GetByIdAsync(id) == null)
                return NotFound($"Эмоция с ID {id} не существует.");

            await emotionRepository.UpdateAsync(id, emotion);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            if (await emotionRepository.GetByIdAsync(id) == null)
                return NotFound($"Эмоция с ID {id} не существует.");

            await emotionRepository.DeleteAsync(id);
            return Ok();
        }
    }
}
