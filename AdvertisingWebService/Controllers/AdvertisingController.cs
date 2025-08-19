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
        /// ����� ��� �������� �������� �� �����
        /// POST /api/v1/advertising/load?filePath=
        /// </summary>
        /// <param name="pathToFile">���� � �����</param>
        /// <returns>���������� ������ ��������</returns>
        [HttpPost("load")]
        public IActionResult Load([FromQuery] string pathToFile) {
            _logger.LogInformation("������ Api ����� Load");
            if (pathToFile==null)
            {
                _logger.LogWarning("�� ������� ���� �� �����");
                return BadRequest("�� ������ ���� � �����");
            }

            try
            {
                _service.LoadFromFile(pathToFile);
                _logger.LogInformation($"�������� ��������� �� ����� {pathToFile}");
                return Ok("������ ������� ���������");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"������ ��� �������� ����� {pathToFile}");
                return StatusCode(500, "������ ��� �������� ������");
            }

        }
        /// <summary>
        /// ����� ��� ������ ��������� �� �������
        /// GET /api/v1/advertising/search?location=
        /// </summary>
        /// <param name="location">�������, ��������: /ru/svrd</param>
        /// <returns></returns>
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string location)
        {
            _logger.LogInformation("������ Api ����� Search");
            if (string.IsNullOrWhiteSpace(location))
            {
                _logger.LogWarning("������ ������ ��� �������� �������");
                return BadRequest("�� ������� �������");
            }

            try
            {
                var platforms = _service.GetPlatformsList(location);
                _logger.LogInformation($"�� ������� {location} ������� {platforms.Count} ��������");

                return Ok(platforms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"������ ��� ������ ��� ������� {location}");
                return StatusCode(500, "������ ��� ������");
            }
        }

        /// <summary>
        /// ����� ��� �������� ������ �� ����� ������������
        /// ���������� ��� �� �� ��� ��������� �� ������� ������ ����� ���� �� �����, ����� ����� ��� ���������� ����� ������� ���
        /// </summary>
        /// <param name="file">���� � ������� text/plain </param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            _logger.LogInformation("������ Api ����� UploadFile");
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("���� �� ��������");
                return BadRequest("���� �� ��������");
            }
                

            await _service.LoadFromFileAsync(file);
            return Ok("���� �������� � ���������");
        }
    }
}
