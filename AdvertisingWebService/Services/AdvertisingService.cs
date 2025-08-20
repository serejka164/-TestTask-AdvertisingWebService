using AdvertisingWebService.Models;

namespace AdvertisingWebService.Services
{
    public interface IAdvertisingService
    {
        void LoadFromFile(string path);
        List<string> GetPlatformsList(string location);
        Task LoadFromFileAsync(IFormFile file);
    }
    public class AdvertisingService : IAdvertisingService
    {
        private readonly List<AdvertisingModel> _adsPlatforms = new List<AdvertisingModel>();
        private readonly ILogger<AdvertisingService> _logger;

        public AdvertisingService(ILogger<AdvertisingService> logger)
        {
            _logger = logger;   
        }

        public void LoadFromFile(string path)
        {
            _logger.LogInformation("Начинаем загрузку платформ из файла");
            var newAdsPlatforms = new List<AdvertisingModel>();

            if (!File.Exists(path))
            {
                _logger.LogWarning($"Нет файла по пути {path}, пропускаем загрузку");
                throw new FileNotFoundException($"Файл не найден по пути {path}");
            }

            foreach (var line in File.ReadLines(path))
            {
                try
                {
                    var splitedLine = line.Split(':', 2);
                    var name = splitedLine[0].Trim();
                    var locations = splitedLine[1].Split(',').Select(x => x.Trim()).ToList();

                    if ((name != null) && (locations.Count() > 0))
                        newAdsPlatforms.Add(new AdvertisingModel { Locations = locations, Name = name });

                    _logger.LogInformation($"Обработали и добавили строку {line}");
                }catch(Exception ex)
                {
                    _logger.LogError($"Не смогли обработать строку {line}, причина {ex.Message}");
                    continue;
                }
            }
            
            _adsPlatforms.Clear();
            _adsPlatforms.AddRange(newAdsPlatforms);
            _logger.LogInformation($"Обработали файл, добавили {newAdsPlatforms.Count()} платформ");
        }

        public List<string> GetPlatformsList(string location)
        {
            _logger.LogInformation($"Получаем список платформ в сервисе");
            if (location == null)
            {
                _logger.LogError($"Пустой поисковой запрос");
                return new List<string>();
            }

            var tempList = _adsPlatforms;

            //var result = new List<string>();
            //foreach (var p in tempList)
            //{
            //    foreach (var loc in p.Locations)
            //    {
            //        if (location.StartsWith(loc))
            //        {
            //            result.Add(p.Name);
            //            break; 
            //        }
            //    }
            //}
            //result = result.Distinct().ToList();

            //Более эфективно будет использовать линкью
            var result = tempList.Where(x=> x.Locations.Any(l=> location.StartsWith(l))).Select(x=>x.Name).Distinct().ToList();
            _logger.LogInformation($"Возвращаем {result.Count()} платформ");
            return result;
        }

        public async Task LoadFromFileAsync(IFormFile file)
        {
            _logger.LogInformation("Начинаем загрузку платформ из файла пользователя");
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл пустой или отсутствует");

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var newAdsPlatforms = new List<AdvertisingModel>();
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                try
                {
                    var splitedLine = line.Split(':', 2);
                    var name = splitedLine[0].Trim();
                    var locations = splitedLine[1].Split(',').Select(x => x.Trim()).ToList();

                    if ((name != null) && (locations.Count() > 0))
                        newAdsPlatforms.Add(new AdvertisingModel { Locations = locations, Name = name });

                    _logger.LogInformation($"Обработали и добавили строку {line}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Не смогли обработать строку {line}, причина {ex.Message}");
                    continue;
                }
            }

            _adsPlatforms.Clear();
            _adsPlatforms.AddRange(newAdsPlatforms);
            _logger.LogInformation($"Обработали файл, добавили {newAdsPlatforms.Count()} платформ");
        }

    }
}
