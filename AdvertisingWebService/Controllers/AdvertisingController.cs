using AdvertisingWebService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingWebService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AdvertisingController : ControllerBase
    {
        private readonly AdvertisingService _service;
        private readonly ILogger<AdvertisingController> _logger;

        public AdvertisingController(ILogger<AdvertisingController> logger, AdvertisingService service)
        {
            _logger = logger;
            _service = service;
        }
        /// <summary>
        /// Метод для загрузки платформ из файла
        /// POST /api/v1/advertising/load?filePath=
        /// </summary>
        /// <param name="pathToFile">Путь к файлу</param>
        /// <returns>Возвращает статус операции</returns>
        [HttpPost("load")]
        public IActionResult Load([FromQuery] string pathToFile) {
            _logger.LogInformation("ВызваН Api метод Load");
            if (pathToFile==null)
            {
                _logger.LogWarning("Не передан путь до файла");
                return BadRequest("Не указан путь к файлу");
            }

            try
            {
                _service.LoadFromFile(pathToFile);
                _logger.LogInformation($"Загрузка завершена из файла {pathToFile}");
                return Ok("Данные успешно загружены");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при загрузке файла {pathToFile}");
                return StatusCode(500, "Ошибка при загрузке данных");
            }

        }
        /// <summary>
        /// Метод для поиска плоащадок по локации
        /// GET /api/v1/advertising/search?location=
        /// </summary>
        /// <param name="location">Локация, например: /ru/svrd</param>
        /// <returns></returns>
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string location)
        {
            _logger.LogInformation("Вызван Api метод Search");
            if (string.IsNullOrWhiteSpace(location))
            {
                _logger.LogWarning("Запрос поиска без указания локации");
                return BadRequest("Не указана локация");
            }

            try
            {
                var platforms = _service.GetPlatformsList(location);
                _logger.LogInformation($"По запросу {location} найдено {platforms.Count} площадок");

                return Ok(platforms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при поиске для локации {location}");
                return StatusCode(500, "Ошибка при поиске");
            }
        }

        /// <summary>
        /// Метод для загрузки данных из файла пользователя
        /// Изначально как то не так воспринял ТЗ поэтому сделал прием пути до файла, потом понял что правильнее будет сделать так
        /// </summary>
        /// <param name="file">Файл в формате text/plain </param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            _logger.LogInformation("ВызваН Api метод UploadFile");
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Файл не загружен");
                return BadRequest("Файл не загружен");
            }
                

            await _service.LoadFromFileAsync(file);
            return Ok("Файл загружен и обработан");
        }
    }
}
