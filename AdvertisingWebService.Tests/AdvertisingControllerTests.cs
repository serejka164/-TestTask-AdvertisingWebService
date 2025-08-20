using AdvertisingWebService.Controllers;
using AdvertisingWebService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdvertisingWebService.Tests
{
    public class AdvertisingControllerTests
    {
        private readonly Mock<IAdvertisingService> _serviceMock;
        private readonly Mock<ILogger<AdvertisingController>> _loggerMock;
        private readonly AdvertisingController _controller;
        public AdvertisingControllerTests()
        {
            _serviceMock = new Mock<IAdvertisingService>(); 
            _loggerMock = new Mock<ILogger<AdvertisingController>>();
            _controller = new AdvertisingController(_loggerMock.Object, _serviceMock.Object);
        }


        // ---------- Load ----------
        [Fact]
        public void Load_ShouldReturnBadRequest_WhenPathIsNull()
        {
            var result = _controller.Load(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Не указан путь к файлу", badRequest.Value);
        }

        [Fact]
        public void Load_ShouldCallServiceAndReturnOk_WhenPathIsValid()
        {
            var result = _controller.Load("test.txt");

            _serviceMock.Verify(s => s.LoadFromFile("test.txt"), Times.Once);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Данные успешно загружены", ok.Value);
        }

        [Fact]
        public void Load_ShouldReturn500_WhenServiceThrowsException()
        {
            _serviceMock.Setup(s => s.LoadFromFile(It.IsAny<string>()))
                        .Throws(new Exception("Ошибка загрузки"));

            var result = _controller.Load("bad.txt");

            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
            Assert.Contains("Ошибка при загрузке данных", error.Value.ToString());
        }

        // ---------- Search ----------
        [Fact]
        public void Search_ShouldReturnBadRequest_WhenLocationIsEmpty()
        {
            var result = _controller.Search("");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Не указана локация", badRequest.Value);
        }

        [Fact]
        public void Search_ShouldCallServiceAndReturnOk_WhenLocationIsValid()
        {
            _serviceMock.Setup(s => s.GetPlatformsList("moscow"))
                        .Returns(new List<string> { "A", "B" });

            var result = _controller.Search("moscow");

            var ok = Assert.IsType<OkObjectResult>(result);
            var platforms = Assert.IsAssignableFrom<List<string>>(ok.Value);
            Assert.Equal(2, platforms.Count);
        }

        [Fact]
        public void Search_ShouldReturn500_WhenServiceThrowsException()
        {
            _serviceMock.Setup(s => s.GetPlatformsList(It.IsAny<string>()))
                        .Throws(new Exception("Ошибка поиска"));

            var result = _controller.Search("moscow");

            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
            Assert.Equal("Ошибка при поиске", error.Value);
        }

        // ---------- Upload ----------
        [Fact]
        public async Task UploadFile_ShouldReturnBadRequest_WhenFileIsNull()
        {
            var result = await _controller.UploadFile(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Файл не передан", badRequest.Value);
        }

        [Fact]
        public async Task UploadFile_ShouldReturnBadRequest_WhenFileIsEmpty()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            var result = await _controller.UploadFile(fileMock.Object);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Файл не передан", badRequest.Value);
        }

        [Fact]
        public async Task UploadFile_ShouldCallServiceAndReturnOk_WhenFileIsValid()
        {
            var content = "Яндекс.Директ:/ru\r\nРевдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik\r\nГазета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl\r\nКрутая реклама:/ru/svrd";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns("test.txt");

            var result = await _controller.UploadFile(fileMock.Object);

            _serviceMock.Verify(s => s.LoadFromFileAsync(fileMock.Object), Times.Once);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Данные успешно загружены", ok.Value);
        }
    }
}
